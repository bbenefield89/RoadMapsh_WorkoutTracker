using WorkoutTracker.WebApi.Models.WTUsers;

namespace WorkoutTracker.WebApi.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(string username);

        Task<bool> VerifyHashedPassword(LoginWTUserModel loginUserModel, string providedPassword);
    }
}
