namespace BookingGrid.MainService.Exceptions
{
    /// <summary>Thrown when a requested hotel does not exist.</summary>
    public class HotelNotFoundException : Exception
    {
        public HotelNotFoundException(int id)
            : base($"Hotel with ID {id} not found.") { }
    }
}
