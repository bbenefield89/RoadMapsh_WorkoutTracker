using WorkoutTracker.WebApi.Models.WorkoutDays;

namespace WorkoutTracker.WebApi.Services.Interfaces
{
    public interface IWorkoutDaysService
    {
        Task<WorkoutDayDto?> GetWorkoutDayByDateAndUserId(DateOnly workoutDate, Guid userId);
    }
}
