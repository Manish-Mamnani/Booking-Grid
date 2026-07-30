namespace BookingGrid.AuthService.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException(string msg) : base(msg) { }
    }
}
