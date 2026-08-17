using BookingGrid.MainService.DTOs;
using BookingGrid.MainService.Models;

namespace BookingGrid.MainService.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for dynamic hotel search with filtering, sorting, and pagination.
    /// </summary>
    public interface IHotelQueryRepository
    {
        /// <summary>
        /// Searches approved hotels based on the provided query parameters.
        /// Returns the paged results and the total count before pagination.
        /// </summary>
        Task<(List<Hotel> Hotels, int TotalCount)> SearchAsync(HotelQueryParams queryParams);
    }
}
