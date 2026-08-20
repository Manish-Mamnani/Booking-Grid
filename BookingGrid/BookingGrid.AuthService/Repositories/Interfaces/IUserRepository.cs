using BookingGrid.AuthService.Models;

namespace BookingGrid.AuthService.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for User data access operations.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>Finds a user by its normalized email address.</summary>
        Task<User?> GetByEmailAsync(string email);
        /// <summary>Returns users assigned the specified role.</summary>
        Task<IEnumerable<User>> GetAllByRoleAsync(string role);
        /// <summary>Stages a new user for persistence.</summary>
        Task AddAsync(User user);
        /// <summary>Persists all staged changes.</summary>
        Task SaveChangesAsync();
    }
}
