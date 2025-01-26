using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WorkoutTracker.WebApi.Controllers;
using WorkoutTracker.WebApi.Exceptions;
using WorkoutTracker.WebApi.Models.Responses;
using WorkoutTracker.WebApi.Models.WTUsers;
using WorkoutTracker.WebApi.Services.Interfaces;

namespace WorkoutTracker.Tests.Controllers
{
    public class WTUsersControllerTests
    {
        private readonly Mock<IWTUsersService> _mockWTUsersService;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<ILogger<WTUsersController>> _mockLogger;
        private readonly WTUsersController _controller;

        public WTUsersControllerTests()
        {
            _mockWTUsersService = new Mock<IWTUsersService>();
            _mockJwtService = new Mock<IJwtService>();
            _mockLogger = new Mock<ILogger<WTUsersController>>();
            _controller = new WTUsersController(_mockWTUsersService.Object, _mockLogger.Object, _mockJwtService.Object);
        }

        [Fact]
        public async Task RegisterNewUser_WhenCalled_ShouldReturnCreatedWithUser()
        {
            // Arrange
            var newUser = new CreateWTUserModel { Username = "newuser", Password = "password123" };
            var createdUser = new WTUserDto { Id = Guid.NewGuid(), Username = "newuser" };

            _mockWTUsersService.Setup(s => s.RegisterNewUserAsync(newUser))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _controller.RegisterNewUser(newUser);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<ApiResponseModel<WTUserDto>>(createdAtResult.Value);
            Assert.True(response.Success);
            Assert.Equal("User successfully created.", response.Message);
            Assert.Equal(createdUser.Id, response.Data.Id);
            Assert.Equal(createdUser.Username, response.Data.Username);
        }

        [Fact]
        public async Task Login_WhenCredentialsAreValid_ShouldReturnOkWithToken()
        {
            // Arrange
            var loginModel = new LoginWTUserModel { Username = "validuser", Password = "password123" };
            _mockWTUsersService.Setup(s => s.LogUserIn(loginModel))
                .ReturnsAsync("this_is_a_jwt");

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponseModel<LoginResponseModel>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Authentication successful.", response.Message);
            Assert.Equal("this_is_a_jwt", response.Data.Token);
        }

        [Fact]
        public async Task GetUserById_WhenUserExists_ShouldReturnOkWithUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new WTUserDto { Id = userId, Username = "existinguser" };

            _mockWTUsersService.Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponseModel<WTUserDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("User found.", response.Message);
            Assert.Equal(user.Id, response.Data.Id);
            Assert.Equal(user.Username, response.Data.Username);
        }

        [Fact]
        public void TestProtectedRoute_WhenCalled_ShouldReturnOk()
        {
            // Act
            var result = _controller.TestProtectedRoute();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("This is a protected endpoint!", okResult.Value);
        }

        [Fact]
        public async Task ProblemDetails_WhenCalled_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.ProblemDetails());
        }
    }
}
