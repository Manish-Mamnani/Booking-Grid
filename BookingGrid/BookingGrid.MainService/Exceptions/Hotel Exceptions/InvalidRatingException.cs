namespace BookingGrid.MainService.Exceptions
{
    /// <summary>Thrown when a submitted hotel rating is outside the accepted range.</summary>
    public class InvalidRatingException : Exception
    {
        public InvalidRatingException()
            : base("Rating must be between 1 and 5.") { }
    }
}
