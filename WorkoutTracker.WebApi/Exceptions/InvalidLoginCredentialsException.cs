namespace WorkoutTracker.WebApi.Exceptions
{
    public class InvalidLoginCredentialsException : Exception
    {
        public InvalidLoginCredentialsException()
            : base("Invalid login credentials provided.") 
        { }
    }
}
