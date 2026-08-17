using BookingGrid.MainService.Data;
using BookingGrid.MainService.Models;
using BookingGrid.MainService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingGrid.MainService.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IHotelRepository"/> for hotel and room data access.
    /// </summary>
    public class HotelRepository : IHotelRepository
    {
        private readonly MainDbContext _context;

        public HotelRepository(MainDbContext context)
        {
            _context = context;
        }

        public async Task<Hotel?> GetByIdAsync(int hotelId)
        {
            return await _context.Hotels.FindAsync(hotelId);
        }

        public async Task<Hotel?> GetByIdWithRoomsAsync(int hotelId)
        {
            return await _context.Hotels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.HotelId == hotelId);
        }

        public async Task<List<Hotel>> GetAllWithRoomsAsync()
        {
            return await _context.Hotels
                .Include(h => h.Rooms)
                .OrderByDescending(h => h.HotelId)
                .ToListAsync();
        }

        public async Task<List<Hotel>> GetByStatusWithRoomsAsync(string status)
        {
            return await _context.Hotels
                .Include(h => h.Rooms)
                .Where(h => h.Status == status)
                .ToListAsync();
        }

        public async Task<List<Hotel>> GetByUserIdWithRoomsAsync(int userId)
        {
            return await _context.Hotels
                .Include(h => h.Rooms)
                .Where(h => h.CreatedByUserId == userId)
                .ToListAsync();
        }

        public async Task<Room?> GetRoomByIdWithHotelAsync(int roomId)
        {
            return await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);
        }

        public async Task<List<Room>> GetRoomsByHotelIdAsync(int hotelId)
        {
            return await _context.Rooms
                .Where(r => r.HotelId == hotelId)
                .ToListAsync();
        }

        public async Task<List<int>> GetRoomIdsByUserIdAsync(int userId)
        {
            return await _context.Rooms
                .Where(r => r.Hotel!.CreatedByUserId == userId)
                .Select(r => r.RoomId)
                .ToListAsync();
        }

        public async Task AddHotelAsync(Hotel hotel)
        {
            await _context.Hotels.AddAsync(hotel);
        }

        public async Task AddRoomAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
