namespace WebApplication1.ViewModel
{
    public class CarFilterViewModel
    {
        public string? VehicleType { get; set; }
        public string? Transmission { get; set; }
        public string? FuelType { get; set; }
        public int? Year { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
