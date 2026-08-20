using BookingGrid.MainService.DTOs;

namespace BookingGrid.MainService.Services.Interfaces
{
    /// <summary>Defines review and rating operations for hotels.</summary>
    public interface IReviewService
    {
        /// <summary>Adds a written review for a hotel.</summary>
        Task<ReviewDto> AddReviewAsync(int userId, string userName, CreateReviewDto dto);
        /// <summary>Returns all reviews for a hotel.</summary>
        Task<List<ReviewDto>> GetReviewsByHotelIdAsync(int hotelId);
        /// <summary>Adds a numeric rating and updates the hotel's aggregate rating.</summary>
        Task<ReviewDto> AddRatingAsync(int userId, string userName, int hotelId, int rating);
    }
}
