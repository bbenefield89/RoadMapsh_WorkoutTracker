using WorkoutTracker.WebApi.Data.Entities;
using WorkoutTracker.WebApi.Models.WorkoutDays;

namespace WorkoutTracker.WebApi.Mappers
{
    public static class WorkoutDaysMapper
    {
        public static WorkoutDayDto FromEntityToDto(WorkoutDayEntity entity)
        {
            return new WorkoutDayDto
            {
                Id = entity.Id,
                WorkoutDate = entity.WorkoutDate,
                WorkoutDayExercises = entity.WorkoutDayExercises.Select(i => new WorkoutDayExerciseDto
                {
                    Id = i.Id,
                    Exercise = ExercisesMapper.FromEntityToDto(i.Exercise),
                }).ToList(),
            };
        }
    }
}
