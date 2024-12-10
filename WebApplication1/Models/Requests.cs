using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Requests
    {
        [Key]
        public int RequestId { get; set; }

        public string Userid { get; set; }

        public int Sellid { get; set; }

        public string Sellername { get; set; }

        public string Carname { get; set; }

        public decimal Price { get; set; }

        public RequestStatus status { get; set; } = RequestStatus.Pending;

        public virtual Sell Sell { get; set; }

        public virtual VerificationAppointment VerificationAppointment { get; set; }

    }
}
