using WorkoutTracker.WebApi.Enums;

namespace WorkoutTracker.WebApi.Data.Entities
{
    public class WorkoutDayExerciseEntity
    {
        public Guid Id { get; set; }
        public Guid WorkoutDayId { get; set; }
        public Guid ExerciseId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        // Navigation Properties
        public WorkoutDayEntity WorkoutDay { get; set; }
        public ExerciseEntity Exercise { get; set; }
    }
}
