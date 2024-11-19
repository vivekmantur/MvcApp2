using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModel
{
    public class VerifyOtpViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string OTP { get; set; }
    }
}
