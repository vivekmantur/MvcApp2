using WebApplication1.Models;

namespace WebApplication1.ViewModel
{
    public class CarDashboardViewModel
    {
        public IEnumerable<CarDetails> Cars { get; set; }
        public CarFilterViewModel Filter { get; set; }
    }
}
