using BookingGrid.AuthService.DTOs;

namespace BookingGrid.AuthService.Services.Interfaces
{
    /// <summary>Defines authentication and manager-directory operations.</summary>
    public interface IAuthService
    {
        /// <summary>Registers a user and returns an authenticated session response.</summary>
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        /// <summary>Authenticates a user and returns an authenticated session response.</summary>
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        /// <summary>Returns all users assigned the hotel-manager role.</summary>
        Task<IEnumerable<UserDto>> GetAllManagersAsync();
    }
}
