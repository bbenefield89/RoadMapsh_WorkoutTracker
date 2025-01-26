using WorkoutTracker.WebApi.Data.Entities;

namespace WorkoutTracker.WebApi.Repositories.Interfaces
{
    public interface IWorkoutDaysRepository
    {
        Task<WorkoutDayEntity?> GetWorkoutDayByDateAndUserId(DateOnly workoutDate, Guid userId);
    }
}
