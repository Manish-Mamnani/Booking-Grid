using BookingGrid.MainService.Data;
using BookingGrid.MainService.DTOs;
using BookingGrid.MainService.Models;
using BookingGrid.MainService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingGrid.MainService.Repositories
{
    /// <summary>
    /// EF Core implementation of <see cref="IHotelQueryRepository"/> for dynamic hotel search
    /// with filtering, sorting, and pagination.
    /// </summary>
    public class HotelQueryRepository : IHotelQueryRepository
    {
        private readonly MainDbContext _context;

        public HotelQueryRepository(MainDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Hotel> Hotels, int TotalCount)> SearchAsync(HotelQueryParams queryParams)
        {
            var query = _context.Hotels
                .Include(h => h.Rooms)
                .AsQueryable();

            // Only approved hotels
            query = query.Where(h => h.Status == "Approved");

            // Search by name
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                var search = queryParams.Search.ToLower();
                query = query.Where(h => h.Name.ToLower().Contains(search));
            }

            // City filter
            if (!string.IsNullOrWhiteSpace(queryParams.City))
            {
                var city = queryParams.City.ToLower();
                query = query.Where(h => h.City.ToLower().Contains(city));
            }

            // Min price filter
            if (queryParams.MinPrice.HasValue)
            {
                query = query.Where(h => h.Rooms.Any() &&
                    h.Rooms.Min(r => r.Price) >= queryParams.MinPrice.Value);
            }

            // Max price filter
            if (queryParams.MaxPrice.HasValue)
            {
                query = query.Where(h => h.Rooms.Any() &&
                    h.Rooms.Min(r => r.Price) <= queryParams.MaxPrice.Value);
            }

            // Availability filter
            if (queryParams.AvailableOnly == true)
            {
                query = query.Where(h => h.Rooms.Any(r => r.AvailableCount > 0));
            }

            // Sorting
            query = ApplySorting(query, queryParams.SortBy, queryParams.Order);

            // Total count BEFORE pagination
            var totalCount = await query.CountAsync();

            // Pagination
            var skip = (queryParams.Page - 1) * queryParams.PageSize;
            var hotels = await query.Skip(skip).Take(queryParams.PageSize).ToListAsync();

            return (hotels, totalCount);
        }

        private static IQueryable<Hotel> ApplySorting(IQueryable<Hotel> query, string? sortBy, string? order)
        {
            var isDescending = order?.ToLower() == "desc";

            return sortBy?.ToLower() switch
            {
                "name" => isDescending
                    ? query.OrderByDescending(h => h.Name)
                    : query.OrderBy(h => h.Name),

                "price" => isDescending
                    ? query.OrderByDescending(h => h.Rooms.Any() ? h.Rooms.Min(r => r.Price) : decimal.MinValue)
                    : query.OrderBy(h => h.Rooms.Any() ? h.Rooms.Min(r => r.Price) : decimal.MaxValue),

                "rating" => isDescending
                    ? query.OrderByDescending(h => h.AverageRating)
                    : query.OrderBy(h => h.AverageRating),

                _ => query.OrderBy(h => h.HotelId)
            };
        }
    }
}
