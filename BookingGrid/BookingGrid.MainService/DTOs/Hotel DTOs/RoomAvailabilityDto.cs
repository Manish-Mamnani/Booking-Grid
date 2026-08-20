namespace BookingGrid.MainService.DTOs
{
    /// <summary>Describes a room's available inventory for a requested stay period.</summary>
    public class RoomAvailabilityDto
    {
        public int RoomId { get; set; }
        public int AvailableCount { get; set; }
    }
}
