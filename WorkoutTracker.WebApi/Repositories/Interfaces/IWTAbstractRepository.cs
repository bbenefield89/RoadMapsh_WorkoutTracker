using System.Linq.Expressions;

namespace WorkoutTracker.WebApi.Repositories.Interfaces
{
    public interface IWTAbstractRepository
    {
        Task<bool> DoesEntityExistByConditionAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
    }
}
