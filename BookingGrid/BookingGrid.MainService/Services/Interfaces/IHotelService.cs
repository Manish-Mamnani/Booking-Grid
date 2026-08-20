using BookingGrid.MainService.DTOs;

namespace BookingGrid.MainService.Services.Interfaces
{
    /// <summary>Defines hotel lifecycle and room inventory management operations.</summary>
    public interface IHotelService
    {
        /// <summary>Creates a hotel owned by the specified hotel manager.</summary>
        Task<HotelResponseDto> CreateHotelAsync(int userId, string email, CreateHotelDto dto);
        /// <summary>Updates a hotel after validating owner or administrator access.</summary>
        Task<HotelResponseDto> UpdateHotelAsync(int id, int userId, string role, CreateHotelDto dto);
        /// <summary>Approves a pending hotel for public availability.</summary>
        Task<HotelResponseDto> ApproveHotelAsync(int hotelId);
        /// <summary>Rejects a hotel from the approval workflow.</summary>
        Task<HotelResponseDto> RejectHotelAsync(int hotelId);
        /// <summary>Soft-deletes a hotel after validating owner or administrator access.</summary>
        Task DeleteHotelAsync(int id, int userId, string role);
        /// <summary>Returns an approved hotel by identifier.</summary>
        Task<HotelResponseDto> GetHotelByIdAsync(int id);
        /// <summary>Returns approved hotels for administration.</summary>
        Task<List<HotelResponseDto>> GetApprovedHotelsAsync();
        /// <summary>Returns hotels awaiting administrative approval.</summary>
        Task<List<HotelResponseDto>> GetPendingHotelsAsync();
        /// <summary>Returns hotels owned by a manager.</summary>
        Task<List<HotelResponseDto>> GetMyHotelsAsync(int userId);
        /// <summary>Returns every hotel, regardless of status, for administration.</summary>
        Task<List<HotelResponseDto>> GetAllHotelsAsync();
        /// <summary>Adds an inventory type to an approved hotel owned by the caller.</summary>
        Task<RoomResponseDto> CreateRoomAsync(int userId, CreateRoomDto dto);
        /// <summary>Updates room pricing and capacity while preserving existing reservations.</summary>
        Task<RoomResponseDto> UpdateRoomAsync(int roomId, int userId, UpdateRoomDto dto);
        /// <summary>Returns a room together with its hotel name.</summary>
        Task<RoomResponseDto> GetRoomByIdAsync(int roomId);
        /// <summary>Returns rooms belonging to an approved hotel.</summary>
        Task<List<RoomResponseDto>> GetRoomsByHotelIdAsync(int hotelId);
        /// <summary>Returns identifiers for all rooms owned by a hotel manager.</summary>
        Task<List<int>> GetMyRoomIdsAsync(int userId);
    }
}
