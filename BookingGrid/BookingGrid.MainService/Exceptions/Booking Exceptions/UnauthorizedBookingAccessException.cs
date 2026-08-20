namespace BookingGrid.MainService.Exceptions
{
    /// <summary>Thrown when a caller does not have access to the requested booking.</summary>
    public class UnauthorizedBookingAccessException : Exception
    {
        public UnauthorizedBookingAccessException()
            : base("You are not allowed to perform this action.") { }
    }
}
