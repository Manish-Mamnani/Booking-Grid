namespace BookingGrid.AuthService.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string msg) : base(msg) { }
    }
}
