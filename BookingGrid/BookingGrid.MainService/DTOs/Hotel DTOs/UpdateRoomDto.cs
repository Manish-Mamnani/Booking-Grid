namespace BookingGrid.MainService.DTOs
{
    /// <summary>Request payload used to update a room's price and total inventory.</summary>
    public class UpdateRoomDto
    {
        public decimal Price { get; set; }
        public int TotalCount { get; set; }
    }
}
