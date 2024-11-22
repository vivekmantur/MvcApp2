using System.ComponentModel.DataAnnotations;
using Azure.Identity;
using WebApplication1.Models;

namespace WebApplication1.ViewModel
{
    public class UserViewModel
    {
        [Required]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "The Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}
