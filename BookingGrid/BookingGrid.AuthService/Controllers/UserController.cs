using BookingGrid.AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingGrid.AuthService.Controllers
{
    /// <summary>
    /// Controller for managing user-related operations, primarily for administrative purposes.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("managers")]
        /// <summary>Returns all hotel-manager accounts for an authenticated administrator.</summary>
        public async Task<IActionResult> GetManagers()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return Unauthorized();
            
            var roleClaim = User.FindFirst("Role");
            if (roleClaim == null || roleClaim.Value != "Admin") return Forbid();

            var result = await _authService.GetAllManagersAsync();
            return Ok(result);
        }
    }
}
