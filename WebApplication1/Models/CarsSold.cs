using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class CarsSold
    {
        [Key]
        public int SoldId { get; set; }

        public string UserId { get; set; }

        public int PaymentId { get; set; }

        public int CarId { get; set; }

        public string SellerName { get; set; }

        public string BuyerName { get; set; }

        public string CarName { get; set; }

        public string CarType { get; set; }

        public decimal Price { get; set; }

        public UserRegistration User { get; set; }

        
    }
}
