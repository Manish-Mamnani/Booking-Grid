using BookingGrid.MainService.DTOs;
using BookingGrid.MainService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospitium.BookingService.Controllers
{
    /// <summary>
    /// Controller for managing hotel booking operations, including creation, retrieval, cancellation, and completion.
    /// </summary>
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingController"/> class.
        /// </summary>
        /// <param name="service">The booking service.</param>
        public BookingController(IBookingService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("UserId");
            var emailClaim = User.FindFirst("Email");
            if (userIdClaim == null || emailClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var result = await _service.CreateBookingAsync(userId, emailClaim.Value, dto);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings([FromQuery] string? type)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var result = await _service.GetUserBookingsAsync(userId, type);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllBookings([FromQuery] DateTime? date)
        {
            var roleClaim = User.FindFirst("Role");
            if (roleClaim == null || roleClaim.Value != "Admin") return Forbid();

            var result = await _service.GetAllBookingsAsync(date);
            return Ok(result);
        }

        [Authorize(Roles = "HotelManager")]
        [HttpGet("manager")]
        public async Task<IActionResult> GetManagerBookings()
        {
            var userIdClaim = User.FindFirst("UserId");
            var roleClaim = User.FindFirst("Role");
            
            if (userIdClaim == null) return Unauthorized();
            if (roleClaim == null || roleClaim.Value != "HotelManager") return Forbid();

            var userId = int.Parse(userIdClaim.Value);
            var result = await _service.GetManagerBookingsAsync(userId);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("{bookingId}/cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var userIdClaim = User.FindFirst("UserId");
            var roleClaim = User.FindFirst("Role");
            var emailClaim = User.FindFirst("Email");

            if (userIdClaim == null || roleClaim == null || emailClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var result = await _service.CancelBookingAsync(bookingId, userId, emailClaim.Value, roleClaim.Value);

            return Ok(result);
        }

        [Authorize(Roles = "HotelManager,Admin")]
        [HttpPut("{bookingId}/complete")]
        public async Task<IActionResult> CompleteBooking(int bookingId)
        {
            var userIdClaim = User.FindFirst("UserId");
            var roleClaim = User.FindFirst("Role");

            if (userIdClaim == null || roleClaim == null) return Unauthorized();
            if (roleClaim.Value != "HotelManager" && roleClaim.Value != "Admin") return Forbid();

            var userId = int.Parse(userIdClaim.Value);
            var result = await _service.CompleteBookingAsync(bookingId, userId, roleClaim.Value);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var userIdClaim = User.FindFirst("UserId");
            var roleClaim = User.FindFirst("Role");

            if (userIdClaim == null || roleClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var result = await _service.GetBookingByIdAsync(id, userId, roleClaim.Value);

            return Ok(result);
        }
    }
}