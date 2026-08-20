namespace BookingGrid.MainService.Exceptions
{
    /// <summary>Thrown when a requested room does not exist.</summary>
    public class RoomNotFoundException : Exception
    {
        public RoomNotFoundException(int id)
            : base($"Room with ID {id} not found.") { }
    }
}
