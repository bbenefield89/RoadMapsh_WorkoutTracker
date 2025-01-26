using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using WorkoutTracker.WebApi.Data.Entities;
using WorkoutTracker.WebApi.Models.WTUsers;
using WorkoutTracker.WebApi.Repositories.Interfaces;
using WorkoutTracker.WebApi.Services;

namespace WorkoutTracker.Tests.Services;
public class JwtServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IConfigurationSection> _mockConfigurationSection;
    private readonly Mock<IWTUsersRepository> _mockUsersRepository;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfigurationSection = new Mock<IConfigurationSection>();
        _mockUsersRepository = new Mock<IWTUsersRepository>();

        // Mock JWT settings in configuration
        _mockConfiguration.SetupGet(c => c["Jwt:Key"]).Returns("supersecretkey123456");
        _mockConfiguration.SetupGet(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _mockConfiguration.SetupGet(c => c["Jwt:Audience"]).Returns("TestAudience");
        _mockConfiguration.SetupGet(c => c["Jwt:ExpirationInMinutes"]).Returns("60");

        _jwtService = new JwtService(_mockConfiguration.Object, _mockUsersRepository.Object);
    }

    [Fact]
    public void GenerateJwtToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        var username = "testuser";

        _mockConfigurationSection.Setup(s => s["Key"]).Returns("YourSuperSecretKeyWithAtLeast32Chars!");
        _mockConfigurationSection.Setup(s => s["Issuer"]).Returns("TestIssuer");
        _mockConfigurationSection.Setup(s => s["Audience"]).Returns("TestAudience");
        _mockConfigurationSection.Setup(s => s["ExpirationInMinutes"]).Returns("60");

        _mockConfiguration.Setup(c => c.GetSection("Jwt"))
                          .Returns(_mockConfigurationSection.Object);

        // Act
        var token = _jwtService.GenerateJwtToken(username);

        // Assert
        Assert.NotNull(token);

        // Validate the JWT token structure
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal("TestIssuer", jwtToken.Issuer);
        Assert.Equal("TestAudience", jwtToken.Audiences.First());
        Assert.Equal(username, jwtToken.Subject);
        Assert.NotNull(jwtToken.ValidTo); // Ensure expiration is set
    }

    [Fact]
    public async Task VerifyHashedPassword_WhenPasswordIsValid_ShouldReturnTrue()
    {
        // Arrange
        var loginModel = new LoginWTUserModel { Username = "testuser", Password = "password123" };
        var userEntity = new WTUserEntity { Username = "testuser", Password = "hashedpassword" };

        _mockUsersRepository.Setup(r => r.GetWTUserByUsernameAsync(loginModel.Username))
                            .ReturnsAsync(userEntity);

        var passwordHasher = new PasswordHasher<WTUserEntity>();
        var hashedPassword = passwordHasher.HashPassword(userEntity, "password123");
        userEntity.Password = hashedPassword;

        // Act
        var result = await _jwtService.VerifyHashedPassword(loginModel, "password123");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task VerifyHashedPassword_WhenPasswordIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var loginModel = new LoginWTUserModel { Username = "testuser", Password = "password123" };
        var userEntity = new WTUserEntity { Username = "testuser", Password = "hashedpassword" };

        _mockUsersRepository.Setup(r => r.GetWTUserByUsernameAsync(loginModel.Username))
                            .ReturnsAsync(userEntity);

        var passwordHasher = new PasswordHasher<WTUserEntity>();
        var hashedPassword = passwordHasher.HashPassword(userEntity, "password123");
        userEntity.Password = hashedPassword;

        // Act
        var result = await _jwtService.VerifyHashedPassword(loginModel, "wrongpassword");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task VerifyHashedPassword_WhenUserNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var loginModel = new LoginWTUserModel { Username = "nonexistentuser", Password = "password123" };

        _mockUsersRepository.Setup(r => r.GetWTUserByUsernameAsync(loginModel.Username))
                            .ReturnsAsync((WTUserEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _jwtService.VerifyHashedPassword(loginModel, "password123"));
    }
}
