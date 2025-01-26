namespace WorkoutTracker.WebApi.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string username)
            : base($"A user with the name '{username}' already exists.")
        { }
    }
}
