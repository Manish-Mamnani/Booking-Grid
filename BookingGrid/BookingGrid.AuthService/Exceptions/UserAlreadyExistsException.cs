namespace BookingGrid.AuthService.Exceptions
{
    /// <summary>Thrown when registration attempts to reuse an existing user identity.</summary>
    public class UserAlreadyExistsException : Exception
    {
        /// <summary>Initializes the exception with the duplicate-user message.</summary>
        public UserAlreadyExistsException(string msg) : base(msg) { }
    }
}
