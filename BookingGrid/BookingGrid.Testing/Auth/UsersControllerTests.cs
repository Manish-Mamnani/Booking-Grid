using BookingGrid.AuthService.Controllers;
using BookingGrid.AuthService.DTOs;
using BookingGrid.AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookingGrid.Testing.Auth
{
    [TestFixture]
    public class UsersControllerTests
    {
        private Mock<IAuthService> _authServiceMock;
        private UsersController _controller;

        [SetUp]
        public void SetUp()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new UsersController(_authServiceMock.Object);
        }

        private void SetUserContext(string role)
        {
            var claims = new List<Claim>();
            if (role != null)
            {
                claims.Add(new Claim("UserId", "1"));
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim("Role", role));
            }

            var identity = new ClaimsIdentity(claims, role != null ? "mock" : null);
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task GetManagers_ReturnsOk_WithListOfManagers()
        {
            SetUserContext("Admin");
            _authServiceMock.Setup(s => s.GetAllManagersAsync()).ReturnsAsync(new List<UserDto>());

            var result = await _controller.GetManagers();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetManagers_UserIsNotAdmin_ReturnsForbidden()
        {
            // Arrange
            SetUserContext("User");
            _authServiceMock.Setup(s => s.GetAllManagersAsync()).ReturnsAsync(new List<UserDto>());

            // Act
            var result = await _controller.GetManagers();

            // Assert
            // This is covered by the [Authorize(Roles="Admin")] attribute.
            // Since we are unit testing the controller method directly, it will still execute.
            // Asserting ForbidResult to show failure as requested.
            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task GetManagers_Unauthenticated_ReturnsUnauthorized()
        {
            // Arrange
            SetUserContext(null); // Unauthenticated
            _authServiceMock.Setup(s => s.GetAllManagersAsync()).ReturnsAsync(new List<UserDto>());

            // Act
            var result = await _controller.GetManagers();

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task GetManagers_NoManagersExist_ReturnsOkWithEmptyList()
        {
            // Arrange
            SetUserContext("Admin");
            _authServiceMock.Setup(s => s.GetAllManagersAsync()).ReturnsAsync(new List<UserDto>()); // Empty list

            // Act
            var result = await _controller.GetManagers();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.Empty);
        }
    }
}
