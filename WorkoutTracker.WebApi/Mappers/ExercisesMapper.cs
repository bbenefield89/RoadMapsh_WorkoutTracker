using WorkoutTracker.WebApi.Data.Entities;
using WorkoutTracker.WebApi.Models.Exercises;

namespace WorkoutTracker.WebApi.Mappers
{
    public static class ExercisesMapper
    {
        public static ExerciseDto FromEntityToDto(ExerciseEntity entity)
        {
            return new ExerciseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                MuscleGroup = entity.MuscleGroup,
            };
        }
    }
}
