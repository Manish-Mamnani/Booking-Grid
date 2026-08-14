using BookingGrid.MainService.DTOs;

namespace BookingGrid.MainService.Services.Interfaces
{
    public interface IHotelQueryService
    {
        Task<PaginatedResult<HotelResponseDto>> GetHotelsAsync(HotelQueryParams query);
    }
}
