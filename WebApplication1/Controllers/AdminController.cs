using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;


namespace PrimeCarDeals.Controllers
{
    public class AdminController : Controller
    {
        private readonly WebApplication1Context _context;
        public UserManager<UserRegistration> userManager { get; set; } 
        public AdminController(WebApplication1Context context, UserManager<UserRegistration> userManager)
        {
            _context = context;
            this.userManager = userManager;
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

            return View(request);
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

            
            //var user=UserManager
            CarDetails carDetails = new CarDetails();
            carDetails.UserId = request.Userid;
            carDetails.CarName = request.Carname;
            carDetails.OwnerName = request.Sellername;
            carDetails.Price = request.Price;
            carDetails.Kilometers = request.Sell.Kilometers;
            carDetails.Year = request.Sell.Year;
            carDetails.Variant= request.Sell.Variant;
            carDetails.City = request.Sell.City;
            carDetails.address = request.Sell.Address;
            
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
