namespace BookingGrid.MainService.DTOs
{
    /// <summary>Wraps a page of results together with the total matching-record count.</summary>
    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
