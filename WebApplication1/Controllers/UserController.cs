using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// This class contains the user actions like sell and buy feature
    /// </summary>
    public class UserController : Controller
    {
        public WebApplication1Context context { get; set; }

        public SignInManager<UserRegistration> signInManager;
        public UserController(WebApplication1Context context, SignInManager<UserRegistration> signInManager)
        {
            this.context = context;
            this.signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Sell()
        {
            return View();
        }
        /// <summary>
        /// Car Sell functionality 
        /// </summary>
        /// <param name="sell"></param>
        /// <returns>Dashboard View after filling sell form</returns>
        [HttpPost]
        public IActionResult Sell(SellViewModel sell)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                Sell newSell = new Sell
                {
                    UserId = userId,
                    RcNumber = sell.RcNumber,
                    ChassisNumber = sell.ChassisNumber,
                    Address = sell.Address,
                    Brand = sell.Brand,
                    Transmission = sell.Transmission,
                    FuelType = sell.FuelType,
                    Variant = sell.Variant,
                    Price = sell.Price,
                    CarName = sell.CarName,
                    City = sell.City,
                    OwnerName = sell.OwnerName,
                    Kilometers = sell.Kilometers,
                    Year = sell.Year,
                    VehicleType = sell.VehicleType,
                    LeftImage = ConvertFileToByteArray(sell.LeftImage),
                    RightImage = ConvertFileToByteArray(sell.RightImage),
                    RearImage = ConvertFileToByteArray(sell.RearImage),
                    FrontImage = ConvertFileToByteArray(sell.FrontImage)
                };

                context.Sells.Add(newSell);
                context.SaveChanges(); 
                Requests carSellRequest = new Requests
                {
                    Userid = userId,
                    Sellid = newSell.SellId, 
                    Carname = sell.CarName,
                    Sellername = sell.OwnerName,
                    Price = sell.Price
                };

                context.Requests.Add(carSellRequest);
                context.SaveChanges();

                Console.WriteLine("Request Sent to Admin Wait for Approval");
                return RedirectToAction("Dashboard", "User");
            }
            return View();
        }
        /// <summary>
        /// Convert IformFile to bytearray to insert into database
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private byte[] ConvertFileToByteArray(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public IActionResult Dashboard()
        {
            var unsoldCarList = context.CarDetails.ToList();

            unsoldCarList = unsoldCarList.Where(i => i.Status == "unsold").ToList();

            var model = new CarDashboardViewModel
            {
                Cars = unsoldCarList,
                Filter = new CarFilterViewModel()
            };

            return View(model);
        }

        // Action to filter cars based on the form submission
        [HttpPost]
        public IActionResult FilterCars(CarFilterViewModel filter)
        {
            var filteredCars = context.CarDetails.ToList();
            filteredCars = filteredCars.Where(i => i.Status == "unsold").ToList();
            if (!string.IsNullOrEmpty(filter.VehicleType))
            {
                filteredCars = filteredCars.Where(i => i.VehicleType == filter.VehicleType).ToList();
            }

            if (!string.IsNullOrEmpty(filter.Transmission))
            {
                filteredCars = filteredCars.Where(i => i.Transmission == filter.Transmission).ToList();
            }
            if (!string.IsNullOrEmpty(filter.FuelType))
            {
                filteredCars = filteredCars.Where(i => i.FuelType == filter.FuelType).ToList();
            }
            if (filter.Year.HasValue)
            {
                filteredCars = filteredCars.Where(i => i.Year == filter.Year).ToList();
            }
            if (filter.MinPrice.HasValue)
            {
                filteredCars = filteredCars.Where(i => i.Price >= filter.MinPrice).ToList();
            }
            if (filter.MaxPrice.HasValue)
            {
                filteredCars = filteredCars.Where(i => i.Price <= filter.MaxPrice).ToList();
            }
            var model = new CarDashboardViewModel
            {
                Cars = filteredCars ?? new List<CarDetails>(),
                Filter = filter
            };

            return View("Dashboard", model);
        }
        public IActionResult CarDetails(int id)
        {
            // Finding the car by its ID
            var car = context.CarDetails.FirstOrDefault(c => c.CarId == id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        public IActionResult Book(int id)
        {
            var CarDetails = context.CarDetails.FirstOrDefault(i => i.CarId == id);
            return View(CarDetails);
        }


        public IActionResult MakePayment(int id)
        {

            var carDetails = context.CarDetails.FirstOrDefault(i => i.CarId == id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var amount = carDetails.Price + 10000;
            Payment payment = new Payment();
            payment.CarName = carDetails.CarName;
            payment.SellerName = carDetails.OwnerName;
            payment.AmountPaid = amount;
            payment.BuyyerId = userId;
            payment.CarId = carDetails.CarId;

            context.Payments.Add(payment);

            carDetails.Status = "sold";
            context.Update(carDetails);
            context.SaveChanges();

            //payment details of a particular car
            var carPaymentId = context.Payments.FirstOrDefault(i => i.CarId == id);

            //current login user details
            var currentUser = context.Users.FirstOrDefault(i => i.Id == userId);
            CarsSold carsSold = new CarsSold();
            carsSold.CarId = carDetails.CarId;
            carsSold.PaymentId = carPaymentId.PaymentId;
            carsSold.UserId = carDetails.UserId;
            carsSold.CarName = carDetails.CarName;
            carsSold.BuyerName = currentUser.UserName;
            carsSold.CarType = carDetails.VehicleType;
            carsSold.Price = carDetails.Price;
            carsSold.SellerName = carDetails.OwnerName;
            context.CarsSold.Add(carsSold);
            context.SaveChanges();


            ViewBag.Total = payment.AmountPaid;
            ViewBag.PaymentId = payment.PaymentId;
            return View();


        }
    }
}
