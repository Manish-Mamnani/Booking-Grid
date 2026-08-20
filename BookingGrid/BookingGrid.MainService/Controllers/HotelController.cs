using BookingGrid.MainService.DTOs;
using BookingGrid.MainService.Exceptions;
using BookingGrid.MainService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingGrid.MainService.Controllers
{
    /// <summary>
    /// Controller for managing hotel and room operations, including CRUD, image uploads, and administrative approval workflows.
    /// </summary>
    [ApiController]
    [Route("api/hotels")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly IHotelQueryService _queryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotelController"/> class.
        /// </summary>
        /// <param name="hotelService">The hotel management service.</param>
        /// <param name="queryService">The hotel query and search service.</param>
        public HotelController(IHotelService hotelService, IHotelQueryService queryService)
        {
            _hotelService = hotelService;
            _queryService = queryService;
        }

        [Authorize(Roles = "HotelManager")]
        [HttpPost]
        /// <summary>Creates a hotel owned by the authenticated hotel manager.</summary>
        public async Task<IActionResult> CreateHotel(CreateHotelDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("UserId");
            var emailClaim = User.FindFirst("Email");
            var roleClaim = User.FindFirst("Role");

            if (userIdClaim == null || emailClaim == null) return Unauthorized();
            if (roleClaim == null || roleClaim.Value != "HotelManager") return Forbid();

            var userId = int.Parse(userIdClaim.Value);
            var result = await _hotelService.CreateHotelAsync(userId, emailClaim.Value, dto);
            return Ok(result);
        }

        [Authorize(Roles = "HotelManager")]
        [HttpPost("rooms")]
        /// <summary>Adds room inventory to an approved hotel owned by the caller.</summary>
        public async Task<IActionResult> CreateRoom(CreateRoomDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("UserId");
            var roleClaim = User.FindFirst("Role");

            if (userIdClaim == null) return Unauthorized();
            if (roleClaim == null || roleClaim.Value != "HotelManager") return Forbid();

            var userId = int.Parse(userIdClaim.Value);
            var result = await _hotelService.CreateRoomAsync(userId, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/approve")]
        /// <summary>Approves a hotel for public availability.</summary>
        public async Task<IActionResult> ApproveHotel(int id)
        {
            var roleClaim = User.FindFirst("Role");
            if (roleClaim == null || roleClaim.Value != "Admin") return Forbid();

            var result = await _hotelService.ApproveHotelAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/reject")]
        /// <summary>Rejects a hotel from the approval workflow.</summary>
        public async Task<IActionResult> RejectHotel(int id)
        {
            var roleClaim = User.FindFirst("Role");
            if (roleClaim == null || roleClaim.Value != "Admin") return Forbid();

            var result = await _hotelService.RejectHotelAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("approved")]
        /// <summary>Returns approved hotels for an administrator.</summary>
        public async Task<IActionResult> GetApprovedHotels()
        {
            var roleClaim = User.FindFirst("Role");
            if (roleClaim == null || roleClaim.Value != "Admin") return Forbid();

            var result = await _hotelService.GetApprovedHotelsAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        /// <summary>Returns hotels awaiting approval for an administrator.</summary>
        public async Task<IActionResult> GetPendingHotels()
        {
            var roleClaim = User.FindFirst("Role");
            if (roleClaim == null || roleClaim.Value != "Admin") return Forbid();

            var result = await _hotelService.GetPendingHotelsAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        /// <summary>Returns every hotel, regardless of its workflow status, for an administrator.</summary>
        public async Task<IActionResult> GetAllHotelsForAdmin()
        {
            var roleClaim = User.FindFirst("Role");
            if (roleClaim == null || roleClaim.Value != "Admin") return Forbid();

            var result = await _hotelService.GetAllHotelsAsync();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        /// <summary>Searches and paginates publicly available hotels.</summary>
        public async Task<IActionResult> GetHotels([FromQuery] HotelQueryParams query)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _queryService.GetHotelsAsync(query);
            return Ok(result);
        }

        [HttpGet("rooms/{roomId}")]
        /// <summary>Returns a room by identifier.</summary>
        public async Task<IActionResult> GetRoom(int roomId)
        {
            var result = await _hotelService.GetRoomByIdAsync(roomId);
            return Ok(result);
        }

        [Authorize(Roles = "HotelManager")]
        [HttpPut("rooms/{roomId}")]
        /// <summary>Updates room price and capacity for the authenticated hotel manager.</summary>
        public async Task<IActionResult> UpdateRoom(int roomId, UpdateRoomDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("UserId");
            var roleClaim = User.FindFirst("Role");

            if (userIdClaim == null) return Unauthorized();
            if (roleClaim == null || roleClaim.Value != "HotelManager") return Forbid();

            var userId = int.Parse(userIdClaim.Value);
            var result = await _hotelService.UpdateRoomAsync(roomId, userId, dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        /// <summary>Returns an approved hotel by identifier.</summary>
        public async Task<IActionResult> GetHotel(int id)
        {
            var result = await _hotelService.GetHotelByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("{id}/rooms")]
        /// <summary>Returns rooms belonging to an approved hotel.</summary>
        public async Task<IActionResult> GetRooms(int id)
        {
            var result = await _hotelService.GetRoomsByHotelIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,HotelManager")]
        [HttpPut("{id}")]
        /// <summary>Updates a hotel for its owner or an administrator.</summary>
        public async Task<IActionResult> UpdateHotel(int id, CreateHotelDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("UserId");
            var roleClaim = User.FindFirst("Role");

            if (userIdClaim == null || roleClaim == null) return Unauthorized();
            if (roleClaim.Value != "HotelManager" && roleClaim.Value != "Admin") return Forbid();

            var userId = int.Parse(userIdClaim.Value);
            var result = await _hotelService.UpdateHotelAsync(id, userId, roleClaim.Value, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,HotelManager")]
        [HttpDelete("{id}")]
        /// <summary>Soft-deletes a hotel for its owner or an administrator.</summary>
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var userIdClaim = User.FindFirst("UserId");
            var roleClaim = User.FindFirst("Role");

            if (userIdClaim == null || roleClaim == null) return Unauthorized();
            if (roleClaim.Value != "HotelManager" && roleClaim.Value != "Admin") return Forbid();

            var userId = int.Parse(userIdClaim.Value);
            await _hotelService.DeleteHotelAsync(id, userId, roleClaim.Value);
            return NoContent();
        }

        [Authorize(Roles = "HotelManager")]
        [HttpGet("my")]
        /// <summary>Returns hotels owned by the authenticated hotel manager.</summary>
        public async Task<IActionResult> GetMyHotels()
        {
            var userIdClaim = User.FindFirst("UserId");
            var roleClaim = User.FindFirst("Role");

            if (userIdClaim == null) return Unauthorized();
            if (roleClaim == null || roleClaim.Value != "HotelManager") return Forbid();

            var userId = int.Parse(userIdClaim.Value);
            var result = await _hotelService.GetMyHotelsAsync(userId);
            return Ok(result);
        }

        [Authorize(Roles = "HotelManager")]
        [HttpGet("my/room-ids")]
        /// <summary>Returns identifiers of rooms owned by the authenticated hotel manager.</summary>
        public async Task<IActionResult> GetMyRoomIds()
        {
            var userIdClaim = User.FindFirst("UserId");
            var roleClaim = User.FindFirst("Role");

            if (userIdClaim == null) return Unauthorized();
            if (roleClaim == null || roleClaim.Value != "HotelManager") return Forbid();

            var userId = int.Parse(userIdClaim.Value);
            var result = await _hotelService.GetMyRoomIdsAsync(userId);
            return Ok(result);
        }
    }
}
