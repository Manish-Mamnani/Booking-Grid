using BookingGrid.MainService.Data;
using BookingGrid.MainService.Models;
using BookingGrid.MainService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingGrid.MainService.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IReviewRepository"/> for review data access.
    /// </summary>
    public class ReviewRepository : IReviewRepository
    {
        private readonly MainDbContext _context;

        public ReviewRepository(MainDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetByHotelIdAsync(int hotelId)
        {
            return await _context.Reviews
                .Where(r => r.HotelId == hotelId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
