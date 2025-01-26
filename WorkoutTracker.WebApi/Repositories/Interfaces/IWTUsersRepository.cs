using WorkoutTracker.WebApi.Data.Entities;

namespace WorkoutTracker.WebApi.Repositories.Interfaces
{
    public interface IWTUsersRepository : IWTAbstractRepository
    {
        Task<WTUserEntity?> GetUserByIdAsync(Guid userId);

        Task<WTUserEntity?> GetWTUserByUsernameAsync(string username);

        WTUserEntity CreateWTUser(WTUserEntity user);
    }
}
