using BookingGrid.MainService.Models;

namespace BookingGrid.MainService.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for all booking data access operations.
    /// </summary>
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(int bookingId);
        Task<List<Booking>> GetByUserIdAsync(int userId, string? type);
        Task<List<Booking>> GetAllAsync(DateTime? date);
        Task<List<Booking>> GetByRoomIdsAsync(List<int> roomIds);
        Task<int> GetOverlappingCountAsync(int roomId, DateTime from, DateTime to);

        Task AddAsync(Booking booking);
        Task SaveChangesAsync();
    }
}
