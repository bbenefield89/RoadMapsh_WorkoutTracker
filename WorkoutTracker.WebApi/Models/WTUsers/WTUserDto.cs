namespace WorkoutTracker.WebApi.Models.WTUsers
{
    public class WTUserDto
    {
        public Guid Id { get; set; }

        public required string Username { get; set; }
    }
}
