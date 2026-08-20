using BookingGrid.MainService.Models;

namespace BookingGrid.MainService.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for all booking data access operations.
    /// </summary>
    public interface IBookingRepository
    {
        /// <summary>Finds a booking by identifier.</summary>
        Task<Booking?> GetByIdAsync(int bookingId);
        /// <summary>Returns a user's bookings, optionally filtered by status.</summary>
        Task<List<Booking>> GetByUserIdAsync(int userId, string? type);
        /// <summary>Returns all bookings, optionally filtered by check-in date.</summary>
        Task<List<Booking>> GetAllAsync(DateTime? date);
        /// <summary>Returns bookings for a group of room identifiers.</summary>
        Task<List<Booking>> GetByRoomIdsAsync(List<int> roomIds);
        /// <summary>Counts active bookings that overlap the supplied date range.</summary>
        Task<int> GetOverlappingCountAsync(int roomId, DateTime from, DateTime to);

        /// <summary>Stages a booking for persistence.</summary>
        Task AddAsync(Booking booking);
        /// <summary>Persists all staged changes.</summary>
        Task SaveChangesAsync();
    }
}
