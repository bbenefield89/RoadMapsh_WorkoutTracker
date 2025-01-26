using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WorkoutTracker.WebApi.Data.DbContexts;
using WorkoutTracker.WebApi.Repositories.Interfaces;

namespace WorkoutTracker.WebApi.Repositories
{
    public abstract class WTAbstractRepository : IWTAbstractRepository
    {
        protected readonly WTDbContext _dbContext;

        public WTAbstractRepository(WTDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DoesEntityExistByConditionAsync<T>(Expression<Func<T, bool>> predicate)
            where T : class
        {
            return await _dbContext.Set<T>().AnyAsync(predicate);
        }

    }
}
