using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Net.WebSockets;
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

        public UserManager<UserRegistration> userRegister;
        public UserController(WebApplication1Context context, SignInManager<UserRegistration> signInManager, UserManager<UserRegistration> userRegister)
        {
            this.context = context;
            this.signInManager = signInManager;
            this.userRegister = userRegister;
        }
        public void UpdateLastActivityTime()
        {
            HttpContext.Session.SetString("LastActivity", DateTime.UtcNow.ToString());
        }
        public IActionResult CheckUserInactivity()
        {
            var lastActivity = HttpContext.Session.GetString("LastActivity");
            if (lastActivity != null)
            {
                DateTime lastActivityTime = DateTime.Parse(lastActivity);
                if (DateTime.UtcNow.Subtract(lastActivityTime).TotalMinutes > 2)
                {
                    // User is inactive for more than 10 minutes, log them out
                    signInManager.SignOutAsync().Wait();
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login", "Account");
                }
            }

            // Update the last activity time
            UpdateLastActivityTime();
            return null; // No inactivity detected, continue with the request
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Sell()
        {
            var inactivityCheckResult = CheckUserInactivity();
            if (inactivityCheckResult != null)
            {
                return inactivityCheckResult; // If user is inactive, they'll be redirected to login
            }
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
                    FrontImage = ConvertFileToByteArray(sell.FrontImage),
                    Rc=ConvertFileToByteArray(sell.Rc),
                    Insurance=ConvertFileToByteArray(sell.Insurance)
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

            var inactivityCheckResult = CheckUserInactivity();
            if (inactivityCheckResult != null)
            {
                return inactivityCheckResult; // If user is inactive, they'll be redirected to login
            }
            var unsoldCarList = context.CarDetails.ToList();

            unsoldCarList = unsoldCarList.Where(i => i.Status == "unsold").ToList();

            var model = new CarDashboardViewModel
            {
                Cars = unsoldCarList,
                Filter = new CarFilterViewModel()
            };
            ViewBag.User= User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(model);
        }


        public IActionResult UserPayments(string id)
        {
            //lazy loading
            //var user = userRegister.FindByIdAsync(id);
            //var request = context.Requests.FirstOrDefault(i => i.Userid == id);
            //var payments = request.Sell;
            //return View(payments);


            //eager loading
            var request = context.Requests.Include(r => r.Sell).FirstOrDefault(i=>i.Userid == id);
            return View(request);


            //no - tracking query
            //var request = context.Requests.AsNoTracking().FirstOrDefault(r => r.Userid == id);
            //request.Sellername = "Mani";
            ////context.Requests.Update(request);
            //context.SaveChanges();

            //var request = context.Requests.FirstOrDefault(i => i.Userid == id);
            //request.Sellername = "shyam";
            //var changes = context.ChangeTracker.Entries();
            //foreach (var entry in changes)
            //{
            //    Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
            //}





            return View();
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
            var inactivityCheckResult = CheckUserInactivity();
            if (inactivityCheckResult != null)
            {
                return inactivityCheckResult; // If user is inactive, they'll be redirected to login
            }
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
            payment.PurchaseDate = DateOnly.FromDateTime(DateTime.Now);

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

            List<TestDrive> carTestDrives = context.TestDrives.ToList();
            carTestDrives=carTestDrives.Where(i=>i.CarId== carDetails.CarId).ToList();
            foreach (var car in carTestDrives)
            {
                if (car.Status == TestdriveStatus.Pending)
                {
                    car.Status = TestdriveStatus.Unsuccess;
                    context.Update(car);
                    context.SaveChanges();
                }
            }
            ViewBag.Total = payment.AmountPaid;
            ViewBag.PaymentId = payment.PaymentId;
            return View();

        }
        public IActionResult Chat()
        {
            return View();
        }

        public IActionResult CarStatus()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requests=context.Requests.ToList();
            requests=requests.Where(i=>i.Userid== userId).ToList();
            return View(requests);
        }
        public IActionResult RequestTestDrive(int carId)
        {
            var car = context.CarDetails.FirstOrDefault(c => c.CarId == carId);
            if (car == null || car.Status != "unsold")
            {
                return RedirectToAction("Dashboard", "User"); 
            }

            DateTime today = DateTime.Today;
            List<DateOnly> availableDates = new List<DateOnly>();

            // Add next 6 days (from today)
            for (int i = 0; i < 6; i++)
            {
                availableDates.Add(DateOnly.FromDateTime(today.AddDays(i)));
            }

            List<System.DateOnly> bookedDates = context.TestDrives
                .Where(td => td.CarId == carId)
                .GroupBy(td => td.Testdrivedate)
                .Where(g => g.Count() >= 5)  
                .Select(g => g.Key)
                .ToList();

            String userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<DateOnly> userBookedDates = context.TestDrives
                .Where(td => td.UserId == userId && td.CarId == carId)
                .Select(td => td.Testdrivedate)
                .ToList();

            availableDates = availableDates
                .Where(d => !bookedDates.Contains(d) && !userBookedDates.Contains(d))
                .ToList();

            var model = new TestDriveRequestViewModel
            {
                CarId = carId,
                AvailableDates = availableDates
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult SubmitTestDriveRequest(TestDriveRequestViewModel model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid && model.SelectedDate != null)
            { 
                //checking user selected date testdrive limit
                int bookingsForSelectedDate = context.TestDrives
                    .Count(td => td.CarId == model.CarId && td.Testdrivedate == model.SelectedDate);

                if (bookingsForSelectedDate >= 5)
                {
                    ModelState.AddModelError("SelectedDate", "The selected date has already reached the maximum number of bookings.");
                    return View("RequestTestDrive", model);
                }
                //checking user previously selected that date before
                bool userHasBookedOnThatDate = context.TestDrives
                    .Any(td => td.UserId == userId && td.CarId == model.CarId && td.Testdrivedate == model.SelectedDate);

                if (userHasBookedOnThatDate)
                {
                    ModelState.AddModelError("SelectedDate", "You have already booked a test drive for this date.");
                    return View("RequestTestDrive", model);
                }

                TestDrive testDrive = new TestDrive
                {
                    CarId = model.CarId,
                    UserId = userId,
                    Testdrivedate = model.SelectedDate,
                    Status = TestdriveStatus.Pending
                };

                context.TestDrives.Add(testDrive);
                context.SaveChanges();

                return RedirectToAction("Dashboard", "User");
            }

            return View("RequestTestDrive", model);
        }



        public IActionResult TestDrives()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<TestDrive> testDrives = context.TestDrives.ToList();
            testDrives = testDrives.Where(i => i.UserId == userId).ToList();
            return View(testDrives);

        }
    }
}
