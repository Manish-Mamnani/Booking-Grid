using BookingGrid.MainService.DTOs;
using BookingGrid.MainService.Exceptions;
using BookingGrid.MainService.Models;
using BookingGrid.MainService.Repositories.Interfaces;
using BookingGrid.MainService.Services.Interfaces;

namespace BookingGrid.MainService.Services
{
    /// <summary>
    /// Service implementation for managing the complete booking lifecycle, including availability checks,
    /// conflict resolution, cancellation policies, and event publishing for notifications.
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repo;
        private readonly IHotelService _hotelService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService"/> class.
        /// </summary>
        /// <param name="repo">The booking repository.</param>
        /// <param name="hotelService">The hotel service for room lookups.</param>
        public BookingService(IBookingRepository repo, IHotelService hotelService)
        {
            _repo = repo;
            _hotelService = hotelService;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(int userId, string email, CreateBookingDto dto)
        {
            // Check availability and get room details via Hotel Service
            var room = await _hotelService.GetRoomByIdAsync(dto.RoomId);

            if (room == null || room.AvailableCount < dto.NumberOfRooms)
                throw new RoomNotAvailableException(dto.RoomId);

            // Validate Dates
            if (dto.FromDate >= dto.ToDate)
                throw new InvalidBookingDatesException();

            // Check overlapping bookings and calculate total occupied rooms
            var totalBookedDuringPeriod = await _repo.GetOverlappingCountAsync(dto.RoomId, dto.FromDate, dto.ToDate);

            if (totalBookedDuringPeriod + dto.NumberOfRooms > room.AvailableCount)
                throw new BookingConflictException();

            var days = (dto.ToDate - dto.FromDate).Days;
            if (days < 1) days = 1;

            var booking = new Booking
            {
                UserId = userId,
                RoomId = dto.RoomId,
                HotelName = room.HotelName,
                RoomType = room.Type,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                NumberOfRooms = dto.NumberOfRooms,
                TotalPrice = room.Price * dto.NumberOfRooms * days,
                Status = "Confirmed",
                UserEmail = email
            };

            await _repo.AddAsync(booking);
            await _repo.SaveChangesAsync();

            return await MapToResponse(booking);
        }

        public async Task<List<BookingResponseDto>> GetUserBookingsAsync(int userId, string? type)
        {
            var bookings = await _repo.GetByUserIdAsync(userId, type);

            var dtos = new List<BookingResponseDto>();
            foreach (var b in bookings)
                dtos.Add(await MapToResponse(b));

            return dtos;
        }

        public async Task<List<BookingResponseDto>> GetAllBookingsAsync(DateTime? date)
        {
            var bookings = await _repo.GetAllAsync(date);

            var responseTasks = bookings.Select(b => MapToResponse(b));
            var responseList = await Task.WhenAll(responseTasks);
            return responseList.ToList();
        }

        public async Task<List<BookingResponseDto>> GetManagerBookingsAsync(int userId)
        {
            var roomIds = await _hotelService.GetMyRoomIdsAsync(userId);

            if (roomIds == null || !roomIds.Any())
                return new List<BookingResponseDto>();

            var bookings = await _repo.GetByRoomIdsAsync(roomIds);

            var responseTasks = bookings.Select(b => MapToResponse(b));
            var responseList = await Task.WhenAll(responseTasks);
            return responseList.ToList();
        }

        public async Task<BookingResponseDto> CancelBookingAsync(int id, int userId, string email, string role)
        {
            var booking = await _repo.GetByIdAsync(id);

            if (booking == null)
                throw new BookingNotFoundException(id);

            if (role != "Admin" && booking.UserId != userId)
                throw new UnauthorizedAccessException("You can only cancel your own bookings.");

            if (booking.Status == "Cancelled")
                throw new BookingAlreadyCancelledException(id);

            // Cancellation Policy Logic (Local timezone assumed as reference for check-in)
            var checkInTime = booking.FromDate.Date.AddHours(12);
            var now = DateTime.UtcNow;
            var hoursUntilCheckIn = (checkInTime - now).TotalHours;

            decimal deductionPercentage = 0;
            if (hoursUntilCheckIn < 0) deductionPercentage = 100;
            else if (hoursUntilCheckIn < 24) deductionPercentage = 50;
            else if (hoursUntilCheckIn < 72) deductionPercentage = 25;
            else deductionPercentage = 0;

            booking.CancellationDeduction = (booking.TotalPrice * deductionPercentage) / 100;
            booking.RefundAmount = booking.TotalPrice - booking.CancellationDeduction;
            booking.Status = "Cancelled";

            await _repo.SaveChangesAsync();

            return await MapToResponse(booking);
        }

        public async Task<BookingResponseDto> CompleteBookingAsync(int id, int userId, string role)
        {
            var booking = await _repo.GetByIdAsync(id);

            if (booking == null)
                throw new BookingNotFoundException(id);

            // Security: Managers can complete bookings for their hotels
            if (role != "Admin")
            {
                var roomIds = await _hotelService.GetMyRoomIdsAsync(userId);
                if (roomIds == null || !roomIds.Contains(booking.RoomId))
                    throw new UnauthorizedAccessException("You can only complete bookings for your own properties.");
            }

            if (booking.Status == "Completed")
                return await MapToResponse(booking);

            booking.Status = "Completed";
            await _repo.SaveChangesAsync();

            return await MapToResponse(booking);
        }

        public async Task<BookingResponseDto> GetBookingByIdAsync(int id, int userId, string role)
        {
            var booking = await _repo.GetByIdAsync(id);

            if (booking == null)
                throw new BookingNotFoundException(id);

            if (role != "Admin" && booking.UserId != userId)
                throw new UnauthorizedAccessException("You can only view your own bookings.");

            return await MapToResponse(booking);
        }

        private async Task<BookingResponseDto> MapToResponse(Booking b)
        {
            var dto = new BookingResponseDto
            {
                BookingId = b.BookingId,
                UserId = b.UserId,
                UserEmail = b.UserEmail,
                RoomId = b.RoomId,
                HotelName = b.HotelName,
                RoomType = b.RoomType,
                FromDate = b.FromDate,
                ToDate = b.ToDate,
                NumberOfRooms = b.NumberOfRooms,
                TotalPrice = b.TotalPrice,
                Status = b.Status,
                CancellationDeduction = b.CancellationDeduction,
                RefundAmount = b.RefundAmount
            };

            // Fallback for legacy bookings where denormalized fields are empty
            if (string.IsNullOrEmpty(dto.HotelName))
            {
                try
                {
                    var room = await _hotelService.GetRoomByIdAsync(b.RoomId);
                    if (room != null)
                    {
                        dto.HotelName = room.HotelName;
                        dto.RoomType = room.Type;
                    }
                    else
                    {
                        dto.HotelName = "Unknown Hotel";
                        dto.RoomType = "Unknown Room";
                    }
                }
                catch
                {
                    // Fail gracefully for legacy data fetching
                    dto.HotelName = "Unknown Hotel (Fetch Failed)";
                    dto.RoomType = "Unknown Room";
                }
            }

            return dto;
        }
    }
}