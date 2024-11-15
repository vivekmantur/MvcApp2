using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
