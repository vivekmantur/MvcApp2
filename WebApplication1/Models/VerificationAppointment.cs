using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Models
{
    public class VerificationAppointment
    {
        [Key]
        public int VerificationId { get; set; }
        public int RequestId { get; set; }
        public DateOnly VerificationDate { get; set; }
        public string Verified { get; set; } = "Pending";
        public Requests Request { get; set; }
    }
}
