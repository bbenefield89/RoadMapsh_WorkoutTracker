namespace WorkoutTracker.WebApi.Data.Entities
{
    public class WorkoutDayEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateOnly WorkoutDate { get; set; }
        public TimeOnly WorkoutTime { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        // Navigation Properties
        public List<WorkoutDayExerciseEntity> WorkoutDayExercises { get; set; }
        public WTUserEntity User { get; set; }
    }
}
