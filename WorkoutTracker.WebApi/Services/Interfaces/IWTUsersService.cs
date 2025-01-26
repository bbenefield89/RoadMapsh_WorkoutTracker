using WorkoutTracker.WebApi.Models.WTUsers;

namespace WorkoutTracker.WebApi.Services.Interfaces
{
    public interface IWTUsersService
    {
        Task<WTUserDto> GetUserByIdAsync(Guid userId);

        Task<WTUserDto> GetUserByUsernameAsync(string username);

        Task<WTUserDto> RegisterNewUserAsync(CreateWTUserModel user);

        WTUserDto CreateWTUser(CreateWTUserModel createUserModel);

        Task<bool> DoesUserExistByUsernameAsync(string username);

        Task<string> LogUserIn(LoginWTUserModel user);
    }
}
