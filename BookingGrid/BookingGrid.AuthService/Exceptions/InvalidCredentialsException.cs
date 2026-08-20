namespace BookingGrid.AuthService.Exceptions
{
    /// <summary>Thrown when the supplied credentials cannot authenticate a user.</summary>
    public class InvalidCredentialsException : Exception
    {
        /// <summary>Initializes the exception with an explanation of the authentication failure.</summary>
        public InvalidCredentialsException(string msg) : base(msg) { }
    }
}
