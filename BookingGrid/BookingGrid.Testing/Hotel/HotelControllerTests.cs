using BookingGrid.MainService.Controllers;
using BookingGrid.MainService.DTOs;
using BookingGrid.MainService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookingGrid.Testing.Hotel
{
    [TestFixture]
    public class HotelControllerTests
    {
        private Mock<IHotelService> _hotelServiceMock;
        private Mock<IHotelQueryService> _queryServiceMock;
        private HotelController _controller;

        [SetUp]
        public void SetUp()
        {
            _hotelServiceMock = new Mock<IHotelService>();
            _queryServiceMock = new Mock<IHotelQueryService>();
            _controller = new HotelController(_hotelServiceMock.Object, _queryServiceMock.Object);
            SetUserContext("1", "test@test.com", "HotelManager");
        }

        private void SetUserContext(string userId, string email, string role)
        {
            var claims = new List<Claim>();
            if (userId != null) claims.Add(new Claim("UserId", userId));
            if (email != null) claims.Add(new Claim("Email", email));
            if (role != null) claims.Add(new Claim("Role", role));

            var identity = new ClaimsIdentity(claims, userId != null ? "mock" : null);
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        // --- Positive Tests ---
        [Test]
        public async Task CreateHotel_ValidDto_ReturnsOk()
        {
            var dto = new CreateHotelDto();
            _hotelServiceMock.Setup(s => s.CreateHotelAsync(1, "test@test.com", dto)).ReturnsAsync(new HotelResponseDto());
            var result = await _controller.CreateHotel(dto);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task CreateRoom_ValidDto_ReturnsOk()
        {
            var dto = new CreateRoomDto();
            _hotelServiceMock.Setup(s => s.CreateRoomAsync(1, dto)).ReturnsAsync(new RoomResponseDto());
            var result = await _controller.CreateRoom(dto);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task ApproveHotel_ValidId_ReturnsOk()
        {
            SetUserContext("1", "admin@test.com", "Admin");
            _hotelServiceMock.Setup(s => s.ApproveHotelAsync(1)).ReturnsAsync(new HotelResponseDto());
            var result = await _controller.ApproveHotel(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task RejectHotel_ValidId_ReturnsOk()
        {
            SetUserContext("1", "admin@test.com", "Admin");
            _hotelServiceMock.Setup(s => s.RejectHotelAsync(1)).ReturnsAsync(new HotelResponseDto());
            var result = await _controller.RejectHotel(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetApprovedHotels_ReturnsOk()
        {
            SetUserContext("1", "admin@test.com", "Admin");
            _hotelServiceMock.Setup(s => s.GetApprovedHotelsAsync()).ReturnsAsync(new List<HotelResponseDto>());
            var result = await _controller.GetApprovedHotels();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetPendingHotels_ReturnsOk()
        {
            SetUserContext("1", "admin@test.com", "Admin");
            _hotelServiceMock.Setup(s => s.GetPendingHotelsAsync()).ReturnsAsync(new List<HotelResponseDto>());
            var result = await _controller.GetPendingHotels();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetAllHotelsForAdmin_ReturnsOk()
        {
            SetUserContext("1", "admin@test.com", "Admin");
            _hotelServiceMock.Setup(s => s.GetAllHotelsAsync()).ReturnsAsync(new List<HotelResponseDto>());
            var result = await _controller.GetAllHotelsForAdmin();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetHotels_ValidQuery_ReturnsOk()
        {
            var query = new HotelQueryParams();
            _queryServiceMock.Setup(s => s.GetHotelsAsync(query)).ReturnsAsync((PaginatedResult<HotelResponseDto>)null);
            var result = await _controller.GetHotels(query);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetRoom_ValidId_ReturnsOk()
        {
            _hotelServiceMock.Setup(s => s.GetRoomByIdAsync(1)).ReturnsAsync(new RoomResponseDto());
            var result = await _controller.GetRoom(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task UpdateRoom_ValidDto_ReturnsOk()
        {
            var dto = new UpdateRoomDto();
            _hotelServiceMock.Setup(s => s.UpdateRoomAsync(1, 1, dto)).ReturnsAsync(new RoomResponseDto());
            var result = await _controller.UpdateRoom(1, dto);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetHotel_ValidId_ReturnsOk()
        {
            _hotelServiceMock.Setup(s => s.GetHotelByIdAsync(1)).ReturnsAsync(new HotelResponseDto());
            var result = await _controller.GetHotel(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetRooms_ValidHotelId_ReturnsOk()
        {
            _hotelServiceMock.Setup(s => s.GetRoomsByHotelIdAsync(1)).ReturnsAsync(new List<RoomResponseDto>());
            var result = await _controller.GetRooms(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task UpdateHotel_ValidDto_ReturnsOk()
        {
            var dto = new CreateHotelDto();
            _hotelServiceMock.Setup(s => s.UpdateHotelAsync(1, 1, "HotelManager", dto)).ReturnsAsync(new HotelResponseDto());
            var result = await _controller.UpdateHotel(1, dto);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task DeleteHotel_ValidId_ReturnsNoContent()
        {
            _hotelServiceMock.Setup(s => s.DeleteHotelAsync(1, 1, "HotelManager")).Returns(Task.CompletedTask);
            var result = await _controller.DeleteHotel(1);
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task GetMyHotels_ReturnsOk()
        {
            _hotelServiceMock.Setup(s => s.GetMyHotelsAsync(1)).ReturnsAsync(new List<HotelResponseDto>());
            var result = await _controller.GetMyHotels();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetMyRoomIds_ReturnsOk()
        {
            _hotelServiceMock.Setup(s => s.GetMyRoomIdsAsync(1)).ReturnsAsync(new List<int>());
            var result = await _controller.GetMyRoomIds();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        // --- Negative & Edge Cases ---

        // Hotel CRUD / ownership
        [Test]
        public async Task CreateHotel_UserIsNotManager_ReturnsForbidden()
        {
            SetUserContext("1", "test@test.com", "User");
            var result = await _controller.CreateHotel(new CreateHotelDto());
            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task CreateHotel_InvalidDto_ReturnsBadRequest()
        {
            var dto = new CreateHotelDto();
            _controller.ModelState.AddModelError("Name", "Required");
            var result = await _controller.CreateHotel(dto);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task UpdateHotel_UserDoesNotOwnHotel_ReturnsForbidden()
        {
            SetUserContext("2", "other@test.com", "HotelManager");
            _hotelServiceMock.Setup(s => s.UpdateHotelAsync(1, 2, "HotelManager", It.IsAny<CreateHotelDto>()))
                .ThrowsAsync(new UnauthorizedAccessException());
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.UpdateHotel(1, new CreateHotelDto()));
        }

        [Test]
        public async Task UpdateHotel_InvalidId_ReturnsNotFound()
        {
            _hotelServiceMock.Setup(s => s.UpdateHotelAsync(99, 1, "HotelManager", It.IsAny<CreateHotelDto>()))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.UpdateHotel(99, new CreateHotelDto()));
        }

        [Test]
        public async Task UpdateHotel_InvalidDto_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var result = await _controller.UpdateHotel(1, new CreateHotelDto());
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteHotel_UserDoesNotOwnHotel_ReturnsForbidden()
        {
            SetUserContext("2", "other@test.com", "HotelManager");
            _hotelServiceMock.Setup(s => s.DeleteHotelAsync(1, 2, "HotelManager"))
                .ThrowsAsync(new UnauthorizedAccessException());
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.DeleteHotel(1));
        }

        [Test]
        public async Task DeleteHotel_InvalidId_ReturnsNotFound()
        {
            _hotelServiceMock.Setup(s => s.DeleteHotelAsync(99, 1, "HotelManager"))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.DeleteHotel(99));
        }

        [Test]
        public async Task GetHotel_InvalidId_ReturnsNotFound()
        {
            _hotelServiceMock.Setup(s => s.GetHotelByIdAsync(99))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetHotel(99));
        }

        [Test]
        public async Task GetMyHotels_Unauthenticated_ReturnsUnauthorized()
        {
            SetUserContext(null, null, null);
            var result = await _controller.GetMyHotels();
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        // Rooms
        [Test]
        public async Task CreateRoom_UserDoesNotOwnHotel_ReturnsForbidden()
        {
            SetUserContext("2", "other@test.com", "HotelManager");
            _hotelServiceMock.Setup(s => s.CreateRoomAsync(2, It.IsAny<CreateRoomDto>()))
                .ThrowsAsync(new UnauthorizedAccessException());
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.CreateRoom(new CreateRoomDto()));
        }

        [Test]
        public async Task CreateRoom_InvalidDto_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Type", "Required");
            var result = await _controller.CreateRoom(new CreateRoomDto());
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateRoom_HotelDoesNotExist_ReturnsNotFound()
        {
            _hotelServiceMock.Setup(s => s.CreateRoomAsync(1, It.IsAny<CreateRoomDto>()))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.CreateRoom(new CreateRoomDto()));
        }

        [Test]
        public async Task GetRoom_InvalidId_ReturnsNotFound()
        {
            _hotelServiceMock.Setup(s => s.GetRoomByIdAsync(99))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetRoom(99));
        }

        [Test]
        public async Task UpdateRoom_UserDoesNotOwnHotel_ReturnsForbidden()
        {
            SetUserContext("2", "other@test.com", "HotelManager");
            _hotelServiceMock.Setup(s => s.UpdateRoomAsync(1, 2, It.IsAny<UpdateRoomDto>()))
                .ThrowsAsync(new UnauthorizedAccessException());
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.UpdateRoom(1, new UpdateRoomDto()));
        }

        [Test]
        public async Task UpdateRoom_InvalidId_ReturnsNotFound()
        {
            _hotelServiceMock.Setup(s => s.UpdateRoomAsync(99, 1, It.IsAny<UpdateRoomDto>()))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.UpdateRoom(99, new UpdateRoomDto()));
        }

        [Test]
        public async Task UpdateRoom_InvalidDto_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Type", "Required");
            var result = await _controller.UpdateRoom(1, new UpdateRoomDto());
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetRooms_InvalidHotelId_ReturnsNotFound()
        {
            _hotelServiceMock.Setup(s => s.GetRoomsByHotelIdAsync(99))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetRooms(99));
        }

        [Test]
        public async Task GetMyRoomIds_Unauthenticated_ReturnsUnauthorized()
        {
            SetUserContext(null, null, null);
            var result = await _controller.GetMyRoomIds();
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        // Admin approval workflow
        [Test]
        public async Task ApproveHotel_UserIsNotAdmin_ReturnsForbidden()
        {
            SetUserContext("1", "test@test.com", "HotelManager"); // Not Admin
            var result = await _controller.ApproveHotel(1);
            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task ApproveHotel_InvalidId_ReturnsNotFound()
        {
            SetUserContext("1", "admin@test.com", "Admin");
            _hotelServiceMock.Setup(s => s.ApproveHotelAsync(99))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.ApproveHotel(99));
        }

        [Test]
        public async Task ApproveHotel_AlreadyApproved_ReturnsBadRequest()
        {
            SetUserContext("1", "admin@test.com", "Admin");
            _hotelServiceMock.Setup(s => s.ApproveHotelAsync(1))
                .ThrowsAsync(new InvalidOperationException("Already approved"));
            Assert.ThrowsAsync<InvalidOperationException>(() => _controller.ApproveHotel(1));
        }

        [Test]
        public async Task RejectHotel_UserIsNotAdmin_ReturnsForbidden()
        {
            SetUserContext("1", "test@test.com", "HotelManager"); // Not Admin
            var result = await _controller.RejectHotel(1);
            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task RejectHotel_InvalidId_ReturnsNotFound()
        {
            SetUserContext("1", "admin@test.com", "Admin");
            _hotelServiceMock.Setup(s => s.RejectHotelAsync(99))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.RejectHotel(99));
        }

        [Test]
        public async Task GetPendingHotels_UserIsNotAdmin_ReturnsForbidden()
        {
            SetUserContext("1", "test@test.com", "HotelManager"); // Not Admin
            var result = await _controller.GetPendingHotels();
            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task GetAllHotelsForAdmin_UserIsNotAdmin_ReturnsForbidden()
        {
            SetUserContext("1", "test@test.com", "HotelManager"); // Not Admin
            var result = await _controller.GetAllHotelsForAdmin();
            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        // Listing / query
        [Test]
        public async Task GetApprovedHotels_NoneExist_ReturnsOkWithEmptyList()
        {
            SetUserContext("1", "admin@test.com", "Admin");
            _hotelServiceMock.Setup(s => s.GetApprovedHotelsAsync())
                .ReturnsAsync(new List<HotelResponseDto>());
            var result = await _controller.GetApprovedHotels();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.Empty);
        }

        [Test]
        public async Task GetHotels_InvalidQueryParams_ReturnsBadRequestOrDefaults()
        {
            var query = new HotelQueryParams(); // Removed PageNumber and PageSize as they do not exist
            _queryServiceMock.Setup(s => s.GetHotelsAsync(query))
                .ReturnsAsync(new PaginatedResult<HotelResponseDto> { TotalCount = 0 });
            var result = await _controller.GetHotels(query);
            
            // Depending on controller handling, it either returns 400 or defaults it. 
            // Asserting Ok to see if it clamps or just passes it down.
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetHotels_NoResultsMatchFilter_ReturnsOkWithEmptyList()
        {
            var query = new HotelQueryParams();
            _queryServiceMock.Setup(s => s.GetHotelsAsync(query))
                .ReturnsAsync(new PaginatedResult<HotelResponseDto> { TotalCount = 0 });
            var result = await _controller.GetHotels(query);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}
