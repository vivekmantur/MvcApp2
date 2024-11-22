using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Sell
    {
        [Key]
        public int SellId { get; set; }

        public string UserId { get; set; }

        [Required]
        public string OwnerName { get; set; }


        [Required]
        public string CarName { get; set; }

        [Required]
        public string Variant { get; set; }

        [Required]
        public string Transmission { get; set; }

        [Required]
        public string FuelType { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public string VehicleType { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public long ChassisNumber { get; set; }

        [Required]
        public decimal Kilometers { get; set; }

        [Required]
        public string RcNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]

        // Images (stored as byte arrays)
        public byte[] RearImage { get; set; }
        [Required]
        public byte[] FrontImage { get; set; }
        [Required]
        public byte[] LeftImage { get; set; }
        [Required]
        public byte[] RightImage { get; set; }
        public UserRegistration User { get; set; }

        public Requests Requests { get; set; }
    }
}
