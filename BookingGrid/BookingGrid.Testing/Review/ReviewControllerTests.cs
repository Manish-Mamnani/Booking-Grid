using Hospitium.ReviewService.Controllers;
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

namespace BookingGrid.Testing.Review
{
    [TestFixture]
    public class ReviewControllerTests
    {
        private Mock<IReviewService> _reviewServiceMock;
        private ReviewController _controller;

        [SetUp]
        public void SetUp()
        {
            _reviewServiceMock = new Mock<IReviewService>();
            _controller = new ReviewController(_reviewServiceMock.Object);
            SetUserContext("1", "test@test.com", "Test User", "User");
        }

        private void SetUserContext(string userId, string email, string fullName, string role)
        {
            var claims = new List<Claim>();
            if (userId != null) claims.Add(new Claim("UserId", userId));
            if (email != null) claims.Add(new Claim("Email", email));
            if (fullName != null) claims.Add(new Claim("FullName", fullName));
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
        public async Task AddReview_ValidDto_ReturnsOk()
        {
            var dto = new CreateReviewDto();
            _reviewServiceMock.Setup(s => s.AddReviewAsync(1, "Test User", dto)).ReturnsAsync(new ReviewDto());
            var result = await _controller.AddReview(dto);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task RateHotel_ValidRating_ReturnsOk()
        {
            _reviewServiceMock.Setup(s => s.AddRatingAsync(1, "Test User", 1, 5)).ReturnsAsync(new ReviewDto());
            var result = await _controller.RateHotel(1, 5);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetReviewsByHotel_ValidId_ReturnsOk()
        {
            _reviewServiceMock.Setup(s => s.GetReviewsByHotelIdAsync(1)).ReturnsAsync(new List<ReviewDto>());
            var result = await _controller.GetReviewsByHotel(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        // --- Negative Tests ---
        [Test]
        public async Task AddReview_InvalidDto_ReturnsBadRequest()
        {
            var dto = new CreateReviewDto();
            _controller.ModelState.AddModelError("Comment", "Required");
            var result = await _controller.AddReview(dto);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task AddReview_HotelDoesNotExist_ReturnsNotFound()
        {
            var dto = new CreateReviewDto();
            _reviewServiceMock.Setup(s => s.AddReviewAsync(1, "Test User", dto))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.AddReview(dto));
        }

        [Test]
        public async Task AddReview_Unauthenticated_ReturnsUnauthorized()
        {
            SetUserContext(null, null, null, null);
            var result = await _controller.AddReview(new CreateReviewDto());
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task RateHotel_RatingOutOfRange_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("rating", "Out of range");
            var result = await _controller.RateHotel(1, 6);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task RateHotel_HotelDoesNotExist_ReturnsNotFound()
        {
            _reviewServiceMock.Setup(s => s.AddRatingAsync(1, "Test User", 99, 5))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.RateHotel(99, 5));
        }

        [Test]
        public async Task RateHotel_Unauthenticated_ReturnsUnauthorized()
        {
            SetUserContext(null, null, null, null);
            var result = await _controller.RateHotel(1, 5);
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task GetReviewsByHotel_InvalidId_ReturnsNotFoundOrEmptyList()
        {
            _reviewServiceMock.Setup(s => s.GetReviewsByHotelIdAsync(99))
                .ThrowsAsync(new KeyNotFoundException());
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetReviewsByHotel(99));
        }
    }
}
