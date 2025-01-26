using WorkoutTracker.WebApi.Data.Entities;

namespace WorkoutTracker.WebApi.Models.WorkoutDays
{
    public class WorkoutDayDto
    {
        public Guid Id { get; set; }
        public DateOnly WorkoutDate { get; set; }

        // Navigation Properties
        public List<WorkoutDayExerciseDto> WorkoutDayExercises { get; set; }
    }
}