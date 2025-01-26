namespace WorkoutTracker.WebApi.Data.Entities
{
    public class WTUserEntity
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }

        // Navigation Properties
        public List<WorkoutDayEntity> WorkoutDays { get; set; }
    }
}
