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
        [HttpPost]
        public IActionResult Sell(SellViewModel sell)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                // Create the new Sell entity
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

                // Save the new Sell entity to get its SellId
                context.Sells.Add(newSell);
                context.SaveChanges(); // This saves the Sell and generates the SellId

                // Now create the Request and associate it with the newly created SellId
                Requests carSellRequest = new Requests
                {
                    Userid = userId,
                    Sellid = newSell.SellId, // Use the generated SellId
                    Carname = sell.CarName,
                    Sellername = sell.OwnerName,
                    Price = sell.Price
                };

                // Add the Request and save
                context.Requests.Add(carSellRequest);
                context.SaveChanges();

                Console.WriteLine("Request Sent to Admin Wait for Approval");

                // Redirect after saving both Sell and Request
                return RedirectToAction("Dashboard", "Account");
            }

            // Return the view if the model state is invalid
            return View();
        }

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
    }
}
