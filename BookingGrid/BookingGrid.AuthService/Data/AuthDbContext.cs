using BookingGrid.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingGrid.AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

    }
}
