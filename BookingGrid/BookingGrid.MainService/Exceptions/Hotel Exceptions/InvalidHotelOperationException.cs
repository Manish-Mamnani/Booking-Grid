namespace BookingGrid.MainService.Exceptions
{
    /// <summary>Thrown when a hotel action violates its ownership or workflow rules.</summary>
    public class InvalidHotelOperationException : Exception
    {
        public InvalidHotelOperationException(string message) : base(message) { }
    }
}
