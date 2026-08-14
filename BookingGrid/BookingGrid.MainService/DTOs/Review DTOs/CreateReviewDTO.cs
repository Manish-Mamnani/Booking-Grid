namespace BookingGrid.MainService.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new review submission.
    /// </summary>
    public class CreateReviewDto
    {
        public int HotelId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
