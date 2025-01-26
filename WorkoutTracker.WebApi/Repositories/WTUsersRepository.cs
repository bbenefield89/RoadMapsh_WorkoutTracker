using Microsoft.EntityFrameworkCore;
using WorkoutTracker.WebApi.Data.DbContexts;
using WorkoutTracker.WebApi.Data.Entities;
using WorkoutTracker.WebApi.Repositories.Interfaces;

namespace WorkoutTracker.WebApi.Repositories
{
    public class WTUsersRepository : WTAbstractRepository, IWTUsersRepository
    {
        public WTUsersRepository(WTDbContext dbContext) : base(dbContext)
        { 
        }

        public async Task<WTUserEntity?> GetUserByIdAsync(Guid userId)
        {
            var user = await _dbContext.Users.Where(u => u.Id == userId)
                                             .FirstOrDefaultAsync();

            return user;
        }

        public async Task<WTUserEntity?> GetWTUserByUsernameAsync(string username)
        {
            var user = await _dbContext.Users.Where(u => u.Username == username)
                                             .FirstOrDefaultAsync();

            return user;
        }

        public WTUserEntity CreateWTUser(WTUserEntity user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return user;
        }
    }
}
