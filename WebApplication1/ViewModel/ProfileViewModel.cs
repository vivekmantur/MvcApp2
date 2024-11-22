using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.ViewModel
{
	public class ProfileViewModel
	{
		[EmailAddress]
		public string Email { get; set; }

		public string UserName	{ get; set; }
		[MaxLength(30)]
		public string PhoneNumber { get; set; }

		public string Address	{ get; set; }

		
	}
}
