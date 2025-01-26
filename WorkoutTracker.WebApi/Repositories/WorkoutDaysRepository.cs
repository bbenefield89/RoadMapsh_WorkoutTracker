using Microsoft.EntityFrameworkCore;
using WorkoutTracker.WebApi.Data.DbContexts;
using WorkoutTracker.WebApi.Data.Entities;
using WorkoutTracker.WebApi.Repositories.Interfaces;

namespace WorkoutTracker.WebApi.Repositories
{
    public class WorkoutDaysRepository : IWorkoutDaysRepository
    {
        private readonly WTDbContext _dbContext;

        public WorkoutDaysRepository(WTDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<WorkoutDayEntity?> GetWorkoutDayByDateAndUserId(DateOnly workoutDate, Guid userId)
        {
            var workoutDay = await _dbContext.WorkoutDays
                                             .Include(i => i.WorkoutDayExercises)
                                                 .ThenInclude(i => i.Exercise)
                                             .Where(i => i.WorkoutDate == workoutDate &&
                                                         i.UserId == userId)
                                             .FirstOrDefaultAsync();

            return workoutDay;
        }
    }
}
