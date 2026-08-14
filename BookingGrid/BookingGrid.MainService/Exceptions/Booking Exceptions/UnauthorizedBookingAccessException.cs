namespace BookingGrid.MainService.Exceptions
{
    public class UnauthorizedBookingAccessException : Exception
    {
        public UnauthorizedBookingAccessException()
            : base("You are not allowed to perform this action.") { }
    }
}
