using BookingGrid.AuthService.DTOs;

namespace BookingGrid.AuthService.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<IEnumerable<UserDto>> GetAllManagersAsync();
    }
}
