using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TestDrive
    {
        [Key]
        public int TestDriveId { get; set; }
        public int CarId { get; set; }
        public string UserId { get; set; }
        public DateOnly Testdrivedate { get; set; }

        public TestdriveStatus Status { get; set; } = TestdriveStatus.Pending;
        public UserRegistration User { get; set; }
    }
}
