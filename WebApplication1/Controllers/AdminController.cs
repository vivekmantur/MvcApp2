using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace PrimeCarDeals.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly WebApplication1Context _context;

        public AdminController(WebApplication1Context context)
        {
            _context = context;
        }

        // Action to show all requests (synchronous version)
        public IActionResult Index()
        {
            var requests = _context.Requests
                            .Where(r => r.status == "Pending")
                            .Include(r => r.Sell)
                            .ToList();
            return View(requests);
        }


        // Action to show request details and approve (synchronous version)
        public IActionResult Details(int id)
        {
            var request = _context.Requests
                                  .Include(r => r.Sell) // Include related Sell details
                                  .FirstOrDefault(r => r.RequestId == id);

            if (request == null)
            {
                return NotFound();
            }
            ViewBag.FrontImage = request.Sell.FrontImage;
            ViewBag.RearImage = request.Sell.RearImage;
            ViewBag.LeftImage = request.Sell.LeftImage;
            ViewBag.RightImage = request.Sell.RightImage;
            return View(request);
        }

        public IActionResult Reject(int id)
        {
            var request = _context.Requests.Find(id);
            if (request == null)
            {
                return NotFound();
            }
            request.status = "Rejected";
            _context.Update(request);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        // Action to approve a request and change its status 
        [HttpPost]
        public IActionResult Approve(int id)
        {
            var request = _context.Requests.Find(id);

            if (request == null)
            {
                return NotFound();
            }

            request.status = "Approved";  // Update status to Approved
            _context.Update(request);
            _context.SaveChanges();


            Sell sellDetails = _context.Sells.FirstOrDefault(i => i.SellId == request.Sellid);
            CarDetails carDetails = new CarDetails();
            carDetails.CarName = sellDetails.CarName;
            carDetails.address = sellDetails.Address;
            carDetails.Year = sellDetails.Year;
            carDetails.UserId = sellDetails.UserId;
            carDetails.Kilometers = sellDetails.Kilometers;
            carDetails.OwnerName = sellDetails.OwnerName;
            carDetails.Price = sellDetails.Price;
            carDetails.City = sellDetails.City;
            carDetails.Variant = sellDetails.Variant;
            carDetails.FuelType = sellDetails.FuelType;
            carDetails.Transmission = sellDetails.Transmission;
            carDetails.VehicleType = sellDetails.VehicleType;
            carDetails.FrontImage = sellDetails.FrontImage;
            carDetails.RearImage = sellDetails.RearImage;
            carDetails.LeftImage = sellDetails.LeftImage;
            carDetails.RightImage = sellDetails.RightImage;
            _context.CarDetails.Add(carDetails);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));  // Redirect to the list of requests
        }
        public IActionResult RequestApproved()
        {
            var requests = _context.Requests
                           .Where(r => r.status == "Approved")
                           .Include(r => r.Sell)
                           .ToList();
            return View(requests);
        }
    }
}
