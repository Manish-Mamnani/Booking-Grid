using BookingGrid.MainService.DTOs;

namespace BookingGrid.MainService.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for booking management operations.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>Creates a booking for the authenticated guest.</summary>
        Task<BookingResponseDto> CreateBookingAsync(int userId,string email, CreateBookingDto dto);
        /// <summary>Returns a guest's bookings, optionally restricted by status.</summary>
        Task<List<BookingResponseDto>> GetUserBookingsAsync(int userId, string? type);
        /// <summary>Returns bookings for administrative review, optionally filtered by date.</summary>
        Task<List<BookingResponseDto>> GetAllBookingsAsync(DateTime? date);
        /// <summary>Returns bookings for rooms owned by a hotel manager.</summary>
        Task<List<BookingResponseDto>> GetManagerBookingsAsync(int userId);
        /// <summary>Cancels a booking after verifying the caller's access.</summary>
        Task<BookingResponseDto> CancelBookingAsync(int bookingId, int userId, string email, string role);
        /// <summary>Marks a booking as complete when the caller is authorized.</summary>
        Task<BookingResponseDto> CompleteBookingAsync(int bookingId, int userId, string role);
        /// <summary>Returns a booking after verifying that the caller can view it.</summary>
        Task<BookingResponseDto> GetBookingByIdAsync(int bookingId, int userId, string role);
    }
}
