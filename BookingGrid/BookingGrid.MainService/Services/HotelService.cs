using BookingGrid.MainService.DTOs;
using BookingGrid.MainService.Exceptions;
using BookingGrid.MainService.Models;
using BookingGrid.MainService.Repositories.Interfaces;
using BookingGrid.MainService.Services.Interfaces;

namespace BookingGrid.MainService.Services
{
    /// <summary>
    /// Service implementation for managing hotel and room state, including creation, approval, rejection,
    /// image management, and publishing events for notifications and cross-service communication.
    /// </summary>
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _repo;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotelService"/> class.
        /// </summary>
        /// <param name="repo">The hotel repository.</param>
        public HotelService(IHotelRepository repo)
        {
            _repo = repo;
        }

        public async Task<HotelResponseDto> CreateHotelAsync(int userId, string email, CreateHotelDto dto)
        {
            var hotel = new Hotel
            {
                Name = dto.Name,
                City = dto.City,
                Description = dto.Description ?? string.Empty,
                AverageRating = 0,
                TotalReviews = 0,
                Status = "Pending",
                CreatedByUserId = userId,
                ManagerEmail = email
            };

            await _repo.AddHotelAsync(hotel);
            await _repo.SaveChangesAsync();

            return MapToHotelResponse(hotel);
        }

        public async Task<RoomResponseDto> CreateRoomAsync(int userId, CreateRoomDto dto)
        {
            var hotel = await _repo.GetByIdAsync(dto.HotelId);

            if (hotel == null)
                throw new HotelNotFoundException(dto.HotelId);

            if (hotel.CreatedByUserId != userId)
                throw new UnauthorizedAccessException("You cannot add rooms to this hotel.");

            if (hotel.Status != "Approved")
                throw new InvalidHotelOperationException("Hotel must be approved before adding rooms.");

            var room = new Room
            {
                HotelId = dto.HotelId,
                Type = dto.Type,
                Price = dto.Price,
                TotalCount = dto.TotalCount,
                AvailableCount = dto.TotalCount
            };

            await _repo.AddRoomAsync(room);
            await _repo.SaveChangesAsync();

            return new RoomResponseDto
            {
                RoomId = room.RoomId,
                Type = room.Type,
                Price = room.Price,
                AvailableCount = room.AvailableCount
            };
        }

        public async Task<HotelResponseDto> ApproveHotelAsync(int hotelId)
        {
            var hotel = await _repo.GetByIdWithRoomsAsync(hotelId);

            if (hotel == null)
                throw new HotelNotFoundException(hotelId);

            if (hotel.Status == "Approved")
                throw new InvalidHotelOperationException("Hotel is already approved.");

            hotel.Status = "Approved";
            await _repo.SaveChangesAsync();

            return MapToHotelResponse(hotel);
        }

        public async Task<HotelResponseDto> RejectHotelAsync(int hotelId)
        {
            var hotel = await _repo.GetByIdWithRoomsAsync(hotelId);

            if (hotel == null)
                throw new HotelNotFoundException(hotelId);

            if (hotel.Status == "Rejected")
                throw new InvalidHotelOperationException("Hotel is already rejected.");

            hotel.Status = "Rejected";
            await _repo.SaveChangesAsync();

            return MapToHotelResponse(hotel);
        }

        public async Task<RoomResponseDto> GetRoomByIdAsync(int roomId)
        {
            var room = await _repo.GetRoomByIdWithHotelAsync(roomId);

            if (room == null)
                throw new RoomNotFoundException(roomId);

            return new RoomResponseDto
            {
                RoomId = room.RoomId,
                HotelName = room.Hotel?.Name ?? "Unknown Hotel",
                Type = room.Type,
                Price = room.Price,
                AvailableCount = room.AvailableCount
            };
        }

        public async Task<HotelResponseDto> GetHotelByIdAsync(int id)
        {
            var hotel = await _repo.GetByIdWithRoomsAsync(id);

            if (hotel == null)
                throw new HotelNotFoundException(id);

            if (hotel.Status != "Approved")
                throw new InvalidHotelOperationException("Hotel not available.");

            return MapToHotelResponse(hotel);
        }

        public async Task<List<HotelResponseDto>> GetApprovedHotelsAsync()
        {
            var hotels = await _repo.GetByStatusWithRoomsAsync("Approved");
            return hotels.Select(MapToHotelResponse).ToList();
        }

        public async Task<List<HotelResponseDto>> GetPendingHotelsAsync()
        {
            var hotels = await _repo.GetByStatusWithRoomsAsync("Pending");
            return hotels.Select(MapToHotelResponse).ToList();
        }

        public async Task<List<HotelResponseDto>> GetMyHotelsAsync(int userId)
        {
            var hotels = await _repo.GetByUserIdWithRoomsAsync(userId);
            return hotels.Select(MapToHotelResponse).ToList();
        }

        public async Task<List<int>> GetMyRoomIdsAsync(int userId)
        {
            return await _repo.GetRoomIdsByUserIdAsync(userId);
        }

        public async Task<List<RoomResponseDto>> GetRoomsByHotelIdAsync(int hotelId)
        {
            var hotel = await _repo.GetByIdAsync(hotelId);

            if (hotel == null)
                throw new HotelNotFoundException(hotelId);

            if (hotel.Status != "Approved")
                throw new InvalidHotelOperationException("Hotel not available.");

            var rooms = await _repo.GetRoomsByHotelIdAsync(hotelId);

            return rooms.Select(r => new RoomResponseDto
            {
                RoomId = r.RoomId,
                Type = r.Type,
                Price = r.Price,
                AvailableCount = r.AvailableCount
            }).ToList();
        }

        public async Task<RoomResponseDto> UpdateRoomAsync(int roomId, int userId, UpdateRoomDto dto)
        {
            var room = await _repo.GetRoomByIdWithHotelAsync(roomId);

            if (room == null)
                throw new RoomNotFoundException(roomId);

            var hotel = room.Hotel;

            if (hotel == null)
                throw new InvalidHotelOperationException("Room is not associated with a hotel.");

            if (hotel.CreatedByUserId != userId)
                throw new UnauthorizedAccessException("You cannot update this room.");

            if (hotel.Status != "Approved")
                throw new InvalidHotelOperationException("Cannot update rooms of unapproved hotel.");

            int difference = dto.TotalCount - room.TotalCount;

            room.Price = dto.Price;
            room.TotalCount = dto.TotalCount;
            room.AvailableCount = Math.Min(room.AvailableCount + difference, room.TotalCount);

            if (room.AvailableCount < 0)
                room.AvailableCount = 0;

            await _repo.SaveChangesAsync();

            return new RoomResponseDto
            {
                RoomId = room.RoomId,
                Type = room.Type,
                Price = room.Price,
                AvailableCount = room.AvailableCount
            };
        }

        public async Task<HotelResponseDto> UpdateHotelAsync(int id, int userId, string role, CreateHotelDto dto)
        {
            var hotel = await _repo.GetByIdWithRoomsAsync(id);

            if (hotel == null)
                throw new HotelNotFoundException(id);

            if (hotel.CreatedByUserId != userId && role != "Admin")
                throw new UnauthorizedAccessException("You can only update your own hotels.");

            hotel.Name = dto.Name;
            hotel.City = dto.City;
            hotel.Description = dto.Description ?? string.Empty;

            await _repo.SaveChangesAsync();

            return MapToHotelResponse(hotel);
        }

        public async Task DeleteHotelAsync(int id, int userId, string role)
        {
            var hotel = await _repo.GetByIdAsync(id);

            if (hotel == null)
                throw new HotelNotFoundException(id);

            if (hotel.CreatedByUserId != userId && role != "Admin")
                throw new UnauthorizedAccessException("You can only delete your own hotels.");

            hotel.Status = "Deleted";
            await _repo.SaveChangesAsync();
        }

        public async Task<List<HotelResponseDto>> GetAllHotelsAsync()
        {
            var hotels = await _repo.GetAllWithRoomsAsync();
            return hotels.Select(MapToHotelResponse).ToList();
        }

        private HotelResponseDto MapToHotelResponse(Hotel h)
        {
            return new HotelResponseDto
            {
                HotelId = h.HotelId,
                Name = h.Name,
                City = h.City,
                Status = h.Status,
                ManagerEmail = h.ManagerEmail,
                Description = h.Description,
                Rating = h.AverageRating,
                MinPrice = h.Rooms.Any() ? h.Rooms.Min(r => r.Price) : 0,
            };
        }
    }
}