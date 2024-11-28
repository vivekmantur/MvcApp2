using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace WebApplication1.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }

        
        public IEnumerable<AuthenticationScheme>? ExternalLogins { get; set; }
    }
}
