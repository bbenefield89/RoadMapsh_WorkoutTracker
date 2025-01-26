namespace WorkoutTracker.WebApi.Models.WTUsers
{
    public class CreateWTUserModel
    {
        public required string Username { get; set; }

        public required string Password { get; set; }
    }
}
