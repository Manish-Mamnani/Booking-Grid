using BookingGrid.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingGrid.AuthService.Data
{
    /// <summary>
    /// Entity Framework context for authentication users and their persisted credentials.
    /// </summary>
    public class AuthDbContext : DbContext
    {
        /// <summary>Initializes the context with the configured database options.</summary>
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        /// <summary>Gets the registered user records.</summary>
        public DbSet<User> Users { get; set; }

    }
}
