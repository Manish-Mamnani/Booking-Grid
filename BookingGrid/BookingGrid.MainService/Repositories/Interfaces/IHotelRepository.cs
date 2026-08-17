using BookingGrid.MainService.Models;

namespace BookingGrid.MainService.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for all hotel and room data access operations.
    /// </summary>
    public interface IHotelRepository
    {
        // Hotel queries
        Task<Hotel?> GetByIdAsync(int hotelId);
        Task<Hotel?> GetByIdWithRoomsAsync(int hotelId);
        Task<List<Hotel>> GetAllWithRoomsAsync();
        Task<List<Hotel>> GetByStatusWithRoomsAsync(string status);
        Task<List<Hotel>> GetByUserIdWithRoomsAsync(int userId);

        // Room queries
        Task<Room?> GetRoomByIdWithHotelAsync(int roomId);
        Task<List<Room>> GetRoomsByHotelIdAsync(int hotelId);
        Task<List<int>> GetRoomIdsByUserIdAsync(int userId);

        // Mutations
        Task AddHotelAsync(Hotel hotel);
        Task AddRoomAsync(Room room);
        Task SaveChangesAsync();
    }
}
