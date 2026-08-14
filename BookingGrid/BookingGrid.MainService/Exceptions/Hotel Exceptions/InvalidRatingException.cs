namespace BookingGrid.MainService.Exceptions
{
    public class InvalidRatingException : Exception
    {
        public InvalidRatingException()
            : base("Rating must be between 1 and 5.") { }
    }
}