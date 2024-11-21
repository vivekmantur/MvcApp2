using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public int CarId { get; set; }
        public string BuyyerId { get; set; }
        public string CarName { get; set; }
        public string SellerName  { get; set; }
        public decimal AmountPaid { get; set; }

    }
}
