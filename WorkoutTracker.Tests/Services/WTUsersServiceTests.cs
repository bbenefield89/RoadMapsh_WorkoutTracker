using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using WorkoutTracker.WebApi.Data.Entities;
using WorkoutTracker.WebApi.Exceptions;
using WorkoutTracker.WebApi.Mappers;
using WorkoutTracker.WebApi.Mappers.Interfaces;
using WorkoutTracker.WebApi.Models.WTUsers;
using WorkoutTracker.WebApi.Repositories.Interfaces;
using WorkoutTracker.WebApi.Services;
using WorkoutTracker.WebApi.Services.Interfaces;

namespace WorkoutTracker.Tests.Services;

public class WTUsersServiceTests
{
    private readonly Mock<IWTUsersRepository> _mockRepository;
    private readonly Mock<IPasswordHasher<WTUserEntity>> _mockPasswordHasher;
    private readonly Mock<IWTUsersMapper> _mockUsersMapper;
    private readonly Mock<ILogger<WTUsersService>> _mockLogger;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly WTUsersService _service;

    public WTUsersServiceTests()
    {
        _mockRepository = new Mock<IWTUsersRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher<WTUserEntity>>();
        _mockUsersMapper = new Mock<IWTUsersMapper>();
        _mockLogger = new Mock<ILogger<WTUsersService>>();
        _mockJwtService = new Mock<IJwtService>();

        _service = new WTUsersService(
            _mockRepository.Object,
            _mockPasswordHasher.Object,
            _mockUsersMapper.Object,
            _mockLogger.Object,
            _mockJwtService.Object);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WhenUserExists_ShouldReturnUserDto()
    {
        // Arrange
        var username = "testuser";
        var password = "password";
        var userEntity = new WTUserEntity { Username = username, Password = password };
        var userDto = new WTUserDto { Username = username };

        _mockRepository.Setup(r => r.GetWTUserByUsernameAsync(username))
            .ReturnsAsync(userEntity);

        _mockUsersMapper.Setup(m => m.FromEntityToDto(userEntity))
            .Returns(userDto);

        // Act
        var result = await _service.GetUserByUsernameAsync(username);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(username, result?.Username);
        _mockRepository.Verify(r => r.GetWTUserByUsernameAsync(username), Times.Once);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WhenUserDoesNotExist_ShouldThrow_UserNotFoundException()
    {
        // Arrange
        var username = "nonexistentuser";

        _mockRepository.Setup(r => r.GetWTUserByUsernameAsync(username))
                       .ReturnsAsync((WTUserEntity?)null);

        // Act
        var action = async () => await _service.GetUserByUsernameAsync(username);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(action);
    }

    [Fact]
    public void CreateWTUser_ShouldHashPasswordAndCallRepository()
    {
        // Arrange
        var createUserModel = new CreateWTUserModel { Username = "newuser", Password = "password123" };
        var hashedPassword = "this_is_a_hashed_password";

        var userEntity = new WTUserEntity
        {
            Id = Guid.NewGuid(),
            Username = createUserModel.Username,
            Password = createUserModel.Password,
        };

        _mockUsersMapper.Setup(m => m.FromCreateModelToEntity(createUserModel))
            .Returns(userEntity);

        _mockPasswordHasher.Setup(m => m.HashPassword(userEntity, createUserModel.Password))
            .Returns(hashedPassword);

        _mockRepository.Setup(m => m.CreateWTUser(It.IsAny<WTUserEntity>()));

        // Act
        _service.CreateWTUser(createUserModel);

        // Assert
        //Assert.Equal(hashedPassword, userEntity.Password);

        _mockRepository.Verify(r => r.CreateWTUser(It.Is<WTUserEntity>(
            u => u.Username == userEntity.Username && u.Password == hashedPassword)),
            Times.Once);
    }

    [Fact]
    public async Task DoesUserExistByUsernameAsync_WhenUserExists_ShouldReturnTrue()
    {
        // Arrange
        var username = "existinguser";
        _mockRepository.Setup(r => r.DoesEntityExistByConditionAsync(It.IsAny<Expression<Func<WTUserEntity, bool>>>()))
                       .ReturnsAsync(true);

        // Act
        var result = await _service.DoesUserExistByUsernameAsync(username);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DoesEntityExistByConditionAsync(It.IsAny<Expression<Func<WTUserEntity, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task DoesUserExistByUsernameAsync_WhenUserDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var username = "nonexistentuser";
        _mockRepository.Setup(r => r.DoesEntityExistByConditionAsync(It.IsAny<Expression<Func<WTUserEntity, bool>>>()))
                       .ReturnsAsync(false);

        // Act
        var result = await _service.DoesUserExistByUsernameAsync(username);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.DoesEntityExistByConditionAsync(It.IsAny<Expression<Func<WTUserEntity, bool>>>()), Times.Once);
    }
}
