using BookingGrid.MainService.Data;
using BookingGrid.MainService.DTOs;
using BookingGrid.MainService.Services.Interfaces;
using BookingGrid.MainService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingGrid.MainService.Services
{
    /// <summary>
    /// Service for querying and searching approved hotels with filtering, sorting, and pagination support.
    /// </summary>
    public class HotelQueryService : IHotelQueryService
    {
        private readonly MainDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotelQueryService"/> class.
        /// </summary>
        /// <param name="context">The hotel database context.</param>
        public HotelQueryService(MainDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<HotelResponseDto>> GetHotelsAsync(HotelQueryParams queryParams)
        {
            var query = _context.Hotels
                .Include(h => h.Rooms)
                .AsQueryable();

            //Only approved hotels
            query = query.Where(h => h.Status == "Approved");

            // Search
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

            // Price filter
            if (queryParams.MinPrice.HasValue)
            {
                query = query.Where(h => h.Rooms.Any() &&
                h.Rooms.Min(r => r.Price) >= queryParams.MinPrice.Value);
            }

            if (queryParams.MaxPrice.HasValue)
            {
                query = query.Where(h => h.Rooms.Any() &&
                h.Rooms.Min(r => r.Price) <= queryParams.MaxPrice.Value);
            }

            // Availability
            if (queryParams.AvailableOnly == true)
            {
                query = query.Where(h => h.Rooms.Any(r => r.AvailableCount > 0));
            }

            // Sorting
            query = ApplySorting(query, queryParams.SortBy, queryParams.Order);

            // Total Count (BEFORE pagination)
            var totalCount = await query.CountAsync();

            // Pagination
            var skip = (queryParams.Page - 1) * queryParams.PageSize;
            query = query.Skip(skip).Take(queryParams.PageSize);

            // Projection
            var data = await query.Select(h => new HotelResponseDto
            {
                HotelId = h.HotelId,
                Name = h.Name,
                City = h.City,
                Status = h.Status,
                Rating = h.AverageRating,
                MinPrice = h.Rooms.Any() ? h.Rooms.Min(r => r.Price) : 0,
            }).ToListAsync();

            return new PaginatedResult<HotelResponseDto>
            {
                Data = data,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        private IQueryable<Hotel> ApplySorting(IQueryable<Hotel> query, string? sortBy, string? order)
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