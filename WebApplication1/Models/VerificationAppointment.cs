using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Models
{
    public class VerificationAppointment
    {
        [Key]
        public int VerificationId { get; set; }
        public int RequestId { get; set; }
        public DateOnly VerificationDate { get; set; }
        public VerificationStatus Verified { get; set; } = VerificationStatus.Pending;
        public Requests Request { get; set; }
    }
}
