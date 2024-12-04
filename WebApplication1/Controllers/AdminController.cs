using Azure.Core;
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
        public EmailService emailService;
        public AdminController(WebApplication1Context context,EmailService emailService)
        {
            _context = context;
            this.emailService = emailService;
        }

        // Action to show all requests (synchronous version)
        public IActionResult Index()
        {
            var requests = _context.Requests
                            .Where(r => r.status == RequestStatus.Pending)
                            .Include(r => r.Sell)
                            .ToList();
            return View(requests);
        }

        public void UpdateStatus<T>(T model) where T:class
        {
            var update = _context.Set<T>().Find();
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
            request.status = RequestStatus.Rejected;
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

            request.status = RequestStatus.UnderVerification;  // Update status to Approved
            _context.Update(request);
            _context.SaveChanges();


            //Sell? sellDetails = _context.Sells.FirstOrDefault(i => i.SellId == request.Sellid);
            //CarDetails carDetails = new CarDetails();
            //carDetails.CarName = sellDetails.CarName;
            //carDetails.address = sellDetails.Address;
            //carDetails.Year = sellDetails.Year;
            //carDetails.UserId = sellDetails.UserId;
            //carDetails.Kilometers = sellDetails.Kilometers;
            //carDetails.OwnerName = sellDetails.OwnerName;
            //carDetails.Price = sellDetails.Price;
            //carDetails.City = sellDetails.City;
            //carDetails.Variant = sellDetails.Variant;
            //carDetails.FuelType = sellDetails.FuelType;
            //carDetails.Transmission = sellDetails.Transmission;
            //carDetails.VehicleType = sellDetails.VehicleType;
            //carDetails.FrontImage = sellDetails.FrontImage;
            //carDetails.RearImage = sellDetails.RearImage;
            //carDetails.LeftImage = sellDetails.LeftImage;
            //carDetails.RightImage = sellDetails.RightImage;
            //_context.CarDetails.Add(carDetails);
            //_context.SaveChanges();

            var userid = request.Userid;
            var user = _context.Users.FirstOrDefault(i => i.Id == userid);

            string subject = "Your Car Request Has Been Approved for Verification";
            DateTime appointmentDate = DateTime.Now.AddDays(2); // Two days from today's date

            // Format the appointment date as needed (e.g., in a readable format like "MM/dd/yyyy")
            string formattedAppointmentDate = appointmentDate.ToString("MM/dd/yyyy");

            string message = $@"
                Dear {user.UserName},

                Congratulations! Your car request has been approved for Verification. 

                We have scheduled a vehicle check for the following date:
    
                Appointment Date: {formattedAppointmentDate}

                Please make sure to be available for the appointment. 

                Thank you for choosing PrimeCarDeals!
     
                Regards,
                PrimeCarDeals Team
";

            // Send the email
            emailService.SendEmailAsync(user.Email, subject, message);

            VerificationAppointment verifyCar = new VerificationAppointment();
            verifyCar.VerificationDate = DateOnly.FromDateTime(appointmentDate);
            verifyCar.RequestId = id;
            _context.VerificationAppointments.Add(verifyCar);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));  // Redirect to the list of requests
        }
        public IActionResult ApproveVerification(int id)
        {
            VerificationAppointment? verifyCar = _context.VerificationAppointments.Include(d=>d.Request).FirstOrDefault(i => i.VerificationId == id);
            verifyCar.Verified = VerificationStatus.Verified;
            verifyCar.Request.status =  RequestStatus.Approved;
            _context.Attach(verifyCar.Request);
            _context.Update(verifyCar);
            _context.SaveChanges();

            Sell? sellDetails = _context.Sells.FirstOrDefault(i => i.SellId == verifyCar.Request.Sellid);
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
            return RedirectToAction("RequestApproved");
        }

        public IActionResult GetVerificationPending()
        {
            List<VerificationAppointment> appointments = _context.VerificationAppointments.ToList();
            appointments = appointments.Where(i => i.Verified == VerificationStatus.Pending).ToList();
            return View(appointments);
        }
        public IActionResult RequestApproved()
        {
            var requests = _context.Requests
                           .Where(r => r.status == RequestStatus.Approved)
                           .Include(r => r.Sell)
                           .ToList();
            return View(requests);
        }
        /// <summary>
        /// Action to download pdf
        /// </summary>
        /// <param name="id">Request Id</param>
        /// <returns>pdf File</returns>
        public IActionResult DownloadPdf(int id)
        {
            var request = _context.Requests.Include(r => r.Sell).FirstOrDefault(r => r.RequestId == id);

            if (request != null && request.Sell?.Documents != null)
            {
                return File(request.Sell.Documents, "application/pdf", "DocumentsFile.pdf");
            }

            return NotFound(); 
        }

        public IActionResult ChatAdmin()
        {
            return View();
        }

        
    }
}
