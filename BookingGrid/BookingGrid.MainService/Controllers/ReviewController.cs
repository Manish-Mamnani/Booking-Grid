using BookingGrid.MainService.DTOs;
using BookingGrid.MainService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospitium.ReviewService.Controllers
{
    /// <summary>
    /// Controller for managing hotel reviews and ratings, allowing authenticated users to submit and retrieve reviews.
    /// </summary>
    [ApiController]
    [Route("api/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewController"/> class.
        /// </summary>
        /// <param name="reviewService">The review service.</param>
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize]
        [HttpPost]
        /// <summary>Submits a written review as the authenticated user.</summary>
        public async Task<IActionResult> AddReview(CreateReviewDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var userName = User.FindFirst("FullName")?.Value ?? User.FindFirst("Email")?.Value ?? "Guest";

            var result = await _reviewService.AddReviewAsync(userId, userName, dto);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("rate")]
        /// <summary>Adds an authenticated user's numeric rating to a hotel.</summary>
        public async Task<IActionResult> RateHotel([FromQuery] int hotelId, [FromQuery] int rating)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var userName = User.FindFirst("FullName")?.Value ?? User.FindFirst("Email")?.Value ?? "Guest";

            var result = await _reviewService.AddRatingAsync(userId, userName, hotelId, rating);
            return Ok(result);
        }

        [HttpGet("hotel/{hotelId}")]
        /// <summary>Returns every review submitted for the specified hotel.</summary>
        public async Task<IActionResult> GetReviewsByHotel(int hotelId)
        {
            var result = await _reviewService.GetReviewsByHotelIdAsync(hotelId);
            return Ok(result);
        }
    }
}
