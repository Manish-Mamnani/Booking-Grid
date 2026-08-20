using Microsoft.EntityFrameworkCore;
using BookingGrid.MainService.Models;

namespace BookingGrid.MainService.Data
{
    /// <summary>Entity Framework context for hotels, rooms, bookings, and reviews.</summary>
    public class MainDbContext : DbContext
    {
        /// <summary>Initializes the context with the configured database options.</summary>
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }
        
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
