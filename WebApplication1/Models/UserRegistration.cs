using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;
namespace WebApplication1.Models
{
    public class UserRegistration : IdentityUser
    {
        public string Address { get; set; }
        public ICollection<Sell> Sells { get; set; }
        public ICollection<CarDetails> CarDetails { get; set; } 
        public ICollection<CarsSold> CarsSold { get; set; }
        public ICollection<TestDrive> TestDrives { get; set; }
    }
}