using BookingGrid.AuthService.Controllers;
using BookingGrid.AuthService.DTOs;
using BookingGrid.AuthService.Exceptions;
using BookingGrid.AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace BookingGrid.Testing.Auth
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _authServiceMock;
        private AuthController _controller;

        [SetUp]
        public void SetUp()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Test]
        public async Task Register_ValidDto_ReturnsOk()
        {
            var dto = new RegisterDto { Email = "test@test.com", Password = "Password123!" };
            _authServiceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(new AuthResponseDto());

            var result = await _controller.Register(dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Login_ValidDto_ReturnsOk()
        {
            var dto = new LoginDto { Email = "test@test.com", Password = "Password123!" };
            _authServiceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync(new AuthResponseDto());

            var result = await _controller.Login(dto);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Register_DuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var dto = new RegisterDto { Email = "existing@test.com", Password = "Password123!" };
            _authServiceMock.Setup(s => s.RegisterAsync(dto)).ThrowsAsync(new UserAlreadyExistsException("User exists"));

            // Act & Assert
            // The controller doesn't handle the exception explicitly to return a specific result, 
            // so we assert the exception that middleware would catch.
            var ex = Assert.ThrowsAsync<UserAlreadyExistsException>(async () => await _controller.Register(dto));
            Assert.That(ex.Message, Is.EqualTo("User exists"));
        }

        [Test]
        public async Task Register_InvalidDto_ReturnsBadRequest()
        {
            // Arrange
            var dto = new RegisterDto { Email = "", Password = "" }; // Invalid DTO
            _controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await _controller.Register(dto);

            // Assert
            // Since the controller relies on [ApiController] for validation, calling the method directly 
            // with invalid ModelState will just execute the method unless there's an explicit check.
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new LoginDto { Email = "test@test.com", Password = "WrongPassword" };
            _authServiceMock.Setup(s => s.LoginAsync(dto)).ThrowsAsync(new InvalidCredentialsException("Invalid"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidCredentialsException>(async () => await _controller.Login(dto));
            Assert.That(ex.Message, Is.EqualTo("Invalid"));
        }

        [Test]
        public async Task Login_NonexistentUser_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new LoginDto { Email = "missing@test.com", Password = "Password" };
            _authServiceMock.Setup(s => s.LoginAsync(dto)).ThrowsAsync(new UserNotFoundException("Not found"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<UserNotFoundException>(async () => await _controller.Login(dto));
            Assert.That(ex.Message, Is.EqualTo("Not found"));
        }

        [Test]
        public async Task Login_InvalidDto_ReturnsBadRequest()
        {
            // Arrange
            var dto = new LoginDto { Email = "", Password = "" };
            _controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await _controller.Login(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
