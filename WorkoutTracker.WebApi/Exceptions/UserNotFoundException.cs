namespace WorkoutTracker.WebApi.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(Guid userId)
            : base($"Could not find a user with the id '{userId}'") 
        { }

        public UserNotFoundException(string username)
    : base($"Could not find a user with the username '{username}'")
        { }
    }
}
