using BookingGrid.MainService.DTOs;
using BookingGrid.MainService.Repositories.Interfaces;
using BookingGrid.MainService.Services.Interfaces;

namespace BookingGrid.MainService.Services
{
    /// <summary>
    /// Service for querying and searching approved hotels with filtering, sorting, and pagination support.
    /// </summary>
    public class HotelQueryService : IHotelQueryService
    {
        private readonly IHotelQueryRepository _repo;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotelQueryService"/> class.
        /// </summary>
        /// <param name="repo">The hotel query repository.</param>
        public HotelQueryService(IHotelQueryRepository repo)
        {
            _repo = repo;
        }

        public async Task<PaginatedResult<HotelResponseDto>> GetHotelsAsync(HotelQueryParams queryParams)
        {
            var result = await _repo.SearchAsync(queryParams);
            var hotels    = result.Hotels;      
            var totalCount     = result.TotalCount;  

            var data = hotels.Select(h => new HotelResponseDto
            {
                HotelId = h.HotelId,
                Name = h.Name,
                City = h.City,
                Status = h.Status,
                Rating = h.AverageRating,
                MinPrice = h.Rooms.Any() ? h.Rooms.Min(r => r.Price) : 0,
            }).ToList();

            return new PaginatedResult<HotelResponseDto>
            {
                Data = data,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }
    }
}
