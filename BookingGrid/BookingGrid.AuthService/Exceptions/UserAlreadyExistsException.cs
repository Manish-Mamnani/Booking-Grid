namespace BookingGrid.AuthService.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string msg) : base(msg) { }
    }
}
