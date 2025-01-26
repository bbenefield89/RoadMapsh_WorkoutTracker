using WorkoutTracker.WebApi.Enums;

namespace WorkoutTracker.WebApi.Data.Entities
{
    public class ExerciseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public MuscleGroup MuscleGroup { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        // Navigation Properties
        public List<WorkoutDayExerciseEntity> WorkoutDayExercises { get; set; }
    }
}
