namespace BookingGrid.AuthService.Exceptions
{
    /// <summary>Thrown when an authentication operation cannot find the requested user.</summary>
    public class UserNotFoundException : Exception
    {
        /// <summary>Initializes the exception with the missing-user message.</summary>
        public UserNotFoundException(string msg) : base(msg) { }
    }
}
