using BookingGrid.AuthService.DTOs;
using BookingGrid.AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingGrid.AuthService.Controllers
{
    /// <summary>
    /// Controller for handling authentication-related requests such as login, registration, and password resets.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        /// <summary>Registers a new user and returns a JWT-backed session response.</summary>
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }

        [HttpPost("login")]
        /// <summary>Authenticates the supplied credentials and returns a JWT-backed session response.</summary>
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
    }
}
