using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModel
{
    public class RoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}
