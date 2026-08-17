using BookingGrid.MainService.Data;
using BookingGrid.MainService.Models;
using BookingGrid.MainService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingGrid.MainService.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IBookingRepository"/> for booking data access.
    /// </summary>
    public class BookingRepository : IBookingRepository
    {
        private readonly MainDbContext _context;

        public BookingRepository(MainDbContext context)
        {
            _context = context;
        }

        public async Task<Booking?> GetByIdAsync(int bookingId)
        {
            return await _context.Bookings.FindAsync(bookingId);
        }

        public async Task<List<Booking>> GetByUserIdAsync(int userId, string? type)
        {
            var query = _context.Bookings.Where(b => b.UserId == userId);

            if (!string.IsNullOrEmpty(type))
            {
                if (type == "active")
                    query = query.Where(b => b.Status == "Confirmed");
                else if (type == "past")
                    query = query.Where(b => b.Status == "Cancelled" || b.Status == "Completed");
            }

            return await query.OrderByDescending(b => b.FromDate).ToListAsync();
        }

        public async Task<List<Booking>> GetAllAsync(DateTime? date)
        {
            var query = _context.Bookings.AsQueryable();

            if (date.HasValue)
                query = query.Where(b => b.FromDate.Date == date.Value.Date);

            return await query.OrderByDescending(b => b.BookingId).ToListAsync();
        }

        public async Task<List<Booking>> GetByRoomIdsAsync(List<int> roomIds)
        {
            return await _context.Bookings
                .Where(b => roomIds.Contains(b.RoomId))
                .OrderByDescending(b => b.BookingId)
                .ToListAsync();
        }

        public async Task<int> GetOverlappingCountAsync(int roomId, DateTime from, DateTime to)
        {
            return await _context.Bookings
                .Where(b => b.RoomId == roomId &&
                            b.Status != "Cancelled" &&
                            from < b.ToDate &&
                            to > b.FromDate)
                .SumAsync(b => b.NumberOfRooms);
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
