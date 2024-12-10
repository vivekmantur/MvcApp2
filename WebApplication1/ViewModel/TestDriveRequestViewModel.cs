namespace WebApplication1.ViewModel
{
    public class TestDriveRequestViewModel
    {
        public int CarId { get; set; }
        public List<DateOnly> AvailableDates { get; set; }
        public DateOnly SelectedDate { get; set; }
    }
}
