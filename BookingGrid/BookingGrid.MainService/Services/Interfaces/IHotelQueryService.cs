using BookingGrid.MainService.DTOs;

namespace BookingGrid.MainService.Services.Interfaces
{
    /// <summary>Defines public hotel search, filtering, sorting, and pagination operations.</summary>
    public interface IHotelQueryService
    {
        /// <summary>Returns a page of approved hotels matching the supplied query.</summary>
        Task<PaginatedResult<HotelResponseDto>> GetHotelsAsync(HotelQueryParams query);
    }
}
