using System.Collections.Generic;

namespace WebApplication1.ViewModel
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CurrentRole { get; set; }
        public string NewRole { get; set; }
        public List<string> AvailableRoles { get; set; }
    }
}
