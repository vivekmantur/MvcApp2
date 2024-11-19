using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModel
{
    public class SellViewModel
    {
        [Key]
        public int SellId { get; set; }

        [Required]
        public string OwnerName { get; set; }

        [Required]
        public string CarName { get; set; }

        [Required]
        public string Variant { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public string VehicleType { get; set; }

        [Required]
        public string Transmission { get; set; }

        [Required]
        public string FuelType { get; set; }

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
        public IFormFile RearImage { get; set; }
        [Required]
        public IFormFile FrontImage { get; set; }
        [Required]
        public IFormFile LeftImage { get; set; }
        [Required]
        public IFormFile RightImage { get; set; }
    }
}
