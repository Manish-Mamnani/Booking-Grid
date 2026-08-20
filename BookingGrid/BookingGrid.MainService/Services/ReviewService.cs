using BookingGrid.MainService.DTOs;
using BookingGrid.MainService.Models;
using BookingGrid.MainService.Repositories.Interfaces;
using BookingGrid.MainService.Services.Interfaces;

namespace BookingGrid.MainService.Services
{
    /// <summary>
    /// Service implementation for managing hotel reviews and ratings, persisting data and publishing events to update hotel aggregate ratings.
    /// </summary>
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewService"/> class.
        /// </summary>
        /// <param name="repo">The review repository.</param>
        public ReviewService(IReviewRepository repo)
        {
            _repo = repo;
        }

        public async Task<ReviewDto> AddReviewAsync(int userId, string userName, CreateReviewDto dto)
        {
            var review = new Review
            {
                HotelId = dto.HotelId,
                UserId = userId,
                UserName = userName,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(review);
            await _repo.SaveChangesAsync();

            return MapToDto(review);
        }

        public async Task<ReviewDto> AddRatingAsync(int userId, string userName, int hotelId, int rating)
        {
            var review = new Review
            {
                HotelId = hotelId,
                UserId = userId,
                UserName = userName,
                Rating = rating,
                Comment = "Rating only",
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(review);
            await _repo.SaveChangesAsync();

            return MapToDto(review);
        }

        public async Task<List<ReviewDto>> GetReviewsByHotelIdAsync(int hotelId)
        {
            var reviews = await _repo.GetByHotelIdAsync(hotelId);
            return reviews.Select(MapToDto).ToList();
        }

        /// <summary>Maps a persisted review to its API response representation.</summary>
        private static ReviewDto MapToDto(Review r)
        {
            return new ReviewDto
            {
                ReviewId = r.ReviewId,
                HotelId = r.HotelId,
                UserId = r.UserId,
                UserName = r.UserName,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            };
        }
    }
}
