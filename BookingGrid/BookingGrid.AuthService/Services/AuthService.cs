using BookingGrid.AuthService.DTOs;
using BookingGrid.AuthService.Exceptions;
using BookingGrid.AuthService.Models;
using BookingGrid.AuthService.Repositories.Interfaces;
using BookingGrid.AuthService.Services.Interfaces;

namespace BookingGrid.AuthService.Services
{
    /// <summary>
    /// Service implementation for handling user authentication, registration, and password management.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository for data access.</param>
        /// <param name="jwtService">The JWT service for token generation.</param>
        public AuthService(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
            var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (existingUser != null)
            {
                throw new UserAlreadyExistsException("Email already registered");
            }

            var requestedRole = dto.Role?.Trim();
            // Self-registration can request only the HotelManager role; every other value is a standard user.
            var finalRole = (requestedRole == "HotelManager") ? "HotelManager" : "User";

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = normalizedEmail,
                Role = finalRole,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return CreateAuthResponse(user, "User registered successfully");
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (user == null)
            {
                throw new UserNotFoundException("User not found");
            }

            var validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!validPassword)
            {
                throw new InvalidCredentialsException("Invalid credentials");
            }

            return CreateAuthResponse(user, "Login successful");
        }

        private AuthResponseDto CreateAuthResponse(User user, string message)
        {
            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                Message = message,
                Token = token,
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllManagersAsync()
        {
            var managers = await _userRepository.GetAllByRoleAsync("HotelManager");

            return managers.Select(u => new UserDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            });
        }
    }
}
