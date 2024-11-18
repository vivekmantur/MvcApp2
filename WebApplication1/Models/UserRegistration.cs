using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class UserRegistration:IdentityUser
    {

        public string Address { get; set; }

        
    }
}
