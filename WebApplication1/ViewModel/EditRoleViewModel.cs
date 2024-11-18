using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModel
{
    public class EditRoleViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required(ErrorMessage = "Role Name is Required")]
        public string RoleName { get; set; }
        public List<string>? Users { get; set; }
    }
}
