using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.Models
{
    public class CarDetails
    {
        [Key]
        public int CarId { get; set; }

        public string UserId { get; set; }

        public string CarName { get; set; }

        public string OwnerName { get; set; }

        public decimal Price { get; set; }

        public decimal Kilometers { get; set; }

        public int Year { get; set; }

        public string Variant { get; set; }

        public string address { get; set; }

        public string City { get; set; }

        public string Transmission { get; set; }

        public string FuelType { get; set; }

        public string VehicleType { get; set; }

        public byte[] FrontImage { get; set; }

        public byte[] RearImage { get; set; }

        public byte[] LeftImage { get; set; }

        public byte[] RightImage { get; set; }

        public string Status { get; set; } = "unsold";

        public UserRegistration User { get; set; }


    }
}
