using Hospitium.BookingService.Controllers;
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

namespace BookingGrid.Testing.Booking
{
    [TestFixture]
    public class BookingControllerTests
    {
        private Mock<IBookingService> _bookingServiceMock;
        private BookingController _controller;

        [SetUp]
        public void SetUp()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _controller = new BookingController(_bookingServiceMock.Object);
            SetUserContext("1", "test@test.com", "User");
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
        public async Task CreateBooking_ValidDto_ReturnsOk()
        {
            var dto = new CreateBookingDto();
            _bookingServiceMock.Setup(s => s.CreateBookingAsync(1, "test@test.com", dto)).ReturnsAsync(new BookingResponseDto());
            var result = await _controller.CreateBooking(dto);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetMyBookings_ReturnsOk()
        {
            _bookingServiceMock.Setup(s => s.GetUserBookingsAsync(1, null)).ReturnsAsync(new List<BookingResponseDto>());
            var result = await _controller.GetMyBookings(null);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetAllBookings_ReturnsOk()
        {
            SetUserContext("1", "admin@test.com", "Admin");
            _bookingServiceMock.Setup(s => s.GetAllBookingsAsync(null)).ReturnsAsync(new List<BookingResponseDto>());
            var result = await _controller.GetAllBookings(null);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetManagerBookings_ReturnsOk()
        {
            SetUserContext("1", "manager@test.com", "HotelManager");
            _bookingServiceMock.Setup(s => s.GetManagerBookingsAsync(1)).ReturnsAsync(new List<BookingResponseDto>());
            var result = await _controller.GetManagerBookings();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task CancelBooking_ValidId_ReturnsOk()
        {
            _bookingServiceMock.Setup(s => s.CancelBookingAsync(1, 1, "test@test.com", "User")).ReturnsAsync(new BookingResponseDto());
            var result = await _controller.CancelBooking(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task CompleteBooking_ValidId_ReturnsOk()
        {
            SetUserContext("1", "manager@test.com", "HotelManager");
            _bookingServiceMock.Setup(s => s.CompleteBookingAsync(1, 1, "HotelManager")).ReturnsAsync(new BookingResponseDto());
            var result = await _controller.CompleteBooking(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetBookingById_ValidId_ReturnsOk()
        {
            _bookingServiceMock.Setup(s => s.GetBookingByIdAsync(1, 1, "User")).ReturnsAsync(new BookingResponseDto());
            var result = await _controller.GetBookingById(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        // --- Negative & Edge Cases ---

        // Creation
        [Test]
        public async Task CreateBooking_InvalidDto_ReturnsBadRequest()
        {
            var dto = new CreateBookingDto();
            _controller.ModelState.AddModelError("HotelId", "Required");
            var result = await _controller.CreateBooking(dto);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateBooking_CheckoutBeforeCheckin_ReturnsBadRequest()
        {
            var dto = new CreateBookingDto();
            _bookingServiceMock.Setup(s => s.CreateBookingAsync(It.IsAny<int>(), It.IsAny<string>(), dto))
                .ThrowsAsync(new ArgumentException("Checkout before checkin"));
            Assert.ThrowsAsync<ArgumentException>(() => _controller.CreateBooking(dto));
        }

        [Test]
        public async Task CreateBooking_RoomUnavailableForDates_ReturnsBadRequest()
        {
            var dto = new CreateBookingDto();
            _bookingServiceMock.Setup(s => s.CreateBookingAsync(It.IsAny<int>(), It.IsAny<string>(), dto))
                .ThrowsAsync(new InvalidOperationException("Room unavailable"));
            Assert.ThrowsAsync<InvalidOperationException>(() => _controller.CreateBooking(dto));
        }

        [Test]
        public async Task CreateBooking_RoomDoesNotExist_ReturnsNotFound()
        {
            var dto = new CreateBookingDto();
            _bookingServiceMock.Setup(s => s.CreateBookingAsync(It.IsAny<int>(), It.IsAny<string>(), dto))
                .ThrowsAsync(new KeyNotFoundException("Room not found"));
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.CreateBooking(dto));
        }

        // Authorization boundaries
        [Test]
        public async Task GetMyBookings_Unauthenticated_ReturnsUnauthorized()
        {
            SetUserContext(null, null, null);
            var result = await _controller.GetMyBookings(null);
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task GetAllBookings_UserIsNotAdmin_ReturnsForbidden()
        {
            SetUserContext("1", "test@test.com", "User"); // Not Admin
            var result = await _controller.GetAllBookings(null);
            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task GetManagerBookings_UserIsNotManager_ReturnsForbidden()
        {
            SetUserContext("1", "test@test.com", "User"); // Not HotelManager
            var result = await _controller.GetManagerBookings();
            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        // State/ownership
        [Test]
        public async Task CancelBooking_InvalidId_ReturnsNotFound()
        {
            _bookingServiceMock.Setup(s => s.CancelBookingAsync(99, 1, "test@test.com", "User"))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.CancelBooking(99));
        }

        [Test]
        public async Task CancelBooking_UserDoesNotOwnBooking_ReturnsForbidden()
        {
            _bookingServiceMock.Setup(s => s.CancelBookingAsync(1, 2, "test@test.com", "User"))
                .ThrowsAsync(new UnauthorizedAccessException());
            SetUserContext("2", "test@test.com", "User");
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.CancelBooking(1));
        }

        [Test]
        public async Task CancelBooking_AlreadyCancelledOrCompleted_ReturnsBadRequest()
        {
            _bookingServiceMock.Setup(s => s.CancelBookingAsync(1, 1, "test@test.com", "User"))
                .ThrowsAsync(new InvalidOperationException());
            Assert.ThrowsAsync<InvalidOperationException>(() => _controller.CancelBooking(1));
        }

        [Test]
        public async Task CompleteBooking_InvalidId_ReturnsNotFound()
        {
            SetUserContext("1", "manager@test.com", "HotelManager");
            _bookingServiceMock.Setup(s => s.CompleteBookingAsync(99, 1, "HotelManager"))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.CompleteBooking(99));
        }

        [Test]
        public async Task CompleteBooking_UserNotAuthorized_ReturnsForbidden()
        {
            _bookingServiceMock.Setup(s => s.CompleteBookingAsync(1, 2, "HotelManager"))
                .ThrowsAsync(new UnauthorizedAccessException());
            SetUserContext("2", "manager@test.com", "HotelManager");
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.CompleteBooking(1));
        }

        [Test]
        public async Task GetBookingById_InvalidId_ReturnsNotFound()
        {
            _bookingServiceMock.Setup(s => s.GetBookingByIdAsync(99, 1, "User"))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetBookingById(99));
        }

        [Test]
        public async Task GetBookingById_UserDoesNotOwnBooking_ReturnsForbidden()
        {
            _bookingServiceMock.Setup(s => s.GetBookingByIdAsync(1, 2, "User"))
                .ThrowsAsync(new UnauthorizedAccessException());
            SetUserContext("2", "test@test.com", "User");
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.GetBookingById(1));
        }
    }
}
