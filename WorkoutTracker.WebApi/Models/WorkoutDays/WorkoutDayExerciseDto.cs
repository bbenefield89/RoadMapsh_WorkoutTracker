using WorkoutTracker.WebApi.Data.Entities;
using WorkoutTracker.WebApi.Models.Exercises;

namespace WorkoutTracker.WebApi.Models.WorkoutDays
{
    public class WorkoutDayExerciseDto
    {
        public Guid Id { get; set; }

        // Navigation Properties
        public ExerciseDto Exercise { get; set; }
    }
}
