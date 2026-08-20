using BookingGrid.MainService.Models;

namespace BookingGrid.MainService.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for all review data access operations.
    /// </summary>
    public interface IReviewRepository
    {
        /// <summary>Returns reviews submitted for a hotel.</summary>
        Task<List<Review>> GetByHotelIdAsync(int hotelId);
        /// <summary>Stages a review for persistence.</summary>
        Task AddAsync(Review review);
        /// <summary>Persists all staged changes.</summary>
        Task SaveChangesAsync();
    }
}
