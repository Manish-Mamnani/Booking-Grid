using BookingGrid.AuthService.Data;
using BookingGrid.AuthService.Models;
using BookingGrid.AuthService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingGrid.AuthService.Repositories
{
    /// <summary>
    /// Handles all data access operations for the User entity.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllByRoleAsync(string role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
