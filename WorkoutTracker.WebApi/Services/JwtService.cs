using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkoutTracker.WebApi.Data.Entities;
using WorkoutTracker.WebApi.Models.WTUsers;
using WorkoutTracker.WebApi.Repositories.Interfaces;
using WorkoutTracker.WebApi.Services.Interfaces;

namespace WorkoutTracker.WebApi.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IWTUsersRepository _usersRepository;

        public JwtService(IConfiguration configuration, IWTUsersRepository usersRepository)
        {
            _configuration = configuration;
            _usersRepository = usersRepository;
        }

        public string GenerateJwtToken(string username)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> VerifyHashedPassword(LoginWTUserModel loginUserModel, string providedPassword)
        {
            var userEntity = await _usersRepository.GetWTUserByUsernameAsync(loginUserModel.Username);
            var passwordHasher = new PasswordHasher<WTUserEntity>();

            if (userEntity == null)
            {
                var errorMessage = $"User with username \"{loginUserModel.Username}\" not found";
                throw new KeyNotFoundException(errorMessage);
            }

            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(userEntity,
                                                                                 userEntity.Password,
                                                                                 providedPassword);

            return passwordVerificationResult != PasswordVerificationResult.Failed;
        }
    }
}
