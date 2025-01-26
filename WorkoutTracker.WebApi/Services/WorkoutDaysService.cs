using WorkoutTracker.WebApi.Mappers;
using WorkoutTracker.WebApi.Models.WorkoutDays;
using WorkoutTracker.WebApi.Repositories.Interfaces;
using WorkoutTracker.WebApi.Services.Interfaces;

namespace WorkoutTracker.WebApi.Services
{
    public class WorkoutDaysService : IWorkoutDaysService
    {
        private readonly IWorkoutDaysRepository _workoutDaysRepository;

        public WorkoutDaysService(IWorkoutDaysRepository workoutDaysRepository)
        {
            _workoutDaysRepository = workoutDaysRepository;
        }

        public async Task<WorkoutDayDto?> GetWorkoutDayByDateAndUserId(DateOnly workoutDate, Guid userId)
        {
            var workoutDay = await _workoutDaysRepository.GetWorkoutDayByDateAndUserId(workoutDate, userId);

            return workoutDay == null ?
                null :
                WorkoutDaysMapper.FromEntityToDto(workoutDay);
        }
    }
}
