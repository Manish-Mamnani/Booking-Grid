using BookingGrid.MainService.Models;

namespace BookingGrid.MainService.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for all review data access operations.
    /// </summary>
    public interface IReviewRepository
    {
        Task<List<Review>> GetByHotelIdAsync(int hotelId);
        Task AddAsync(Review review);
        Task SaveChangesAsync();
    }
}
