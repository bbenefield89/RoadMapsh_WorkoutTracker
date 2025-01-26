using WorkoutTracker.WebApi.Enums;
using WorkoutTracker.WebApi.Models.WorkoutDays;

namespace WorkoutTracker.WebApi.Models.Exercises
{
    public class ExerciseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public MuscleGroup MuscleGroup { get; set; }
    }
}
