using BookingGrid.MainService.Models;

namespace BookingGrid.MainService.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for all hotel and room data access operations.
    /// </summary>
    public interface IHotelRepository
    {
        /// <summary>Finds a hotel without loading its rooms.</summary>
        Task<Hotel?> GetByIdAsync(int hotelId);
        /// <summary>Finds a hotel and eagerly loads its rooms.</summary>
        Task<Hotel?> GetByIdWithRoomsAsync(int hotelId);
        /// <summary>Returns all hotels with their rooms.</summary>
        Task<List<Hotel>> GetAllWithRoomsAsync();
        /// <summary>Returns hotels in a given workflow status with their rooms.</summary>
        Task<List<Hotel>> GetByStatusWithRoomsAsync(string status);
        /// <summary>Returns hotels owned by a user with their rooms.</summary>
        Task<List<Hotel>> GetByUserIdWithRoomsAsync(int userId);

        /// <summary>Finds a room and eagerly loads its hotel.</summary>
        Task<Room?> GetRoomByIdWithHotelAsync(int roomId);
        /// <summary>Returns rooms belonging to a hotel.</summary>
        Task<List<Room>> GetRoomsByHotelIdAsync(int hotelId);
        /// <summary>Returns identifiers of rooms owned by a hotel manager.</summary>
        Task<List<int>> GetRoomIdsByUserIdAsync(int userId);

        /// <summary>Stages a hotel for persistence.</summary>
        Task AddHotelAsync(Hotel hotel);
        /// <summary>Stages a room for persistence.</summary>
        Task AddRoomAsync(Room room);
        /// <summary>Persists all staged changes.</summary>
        Task SaveChangesAsync();
    }
}
