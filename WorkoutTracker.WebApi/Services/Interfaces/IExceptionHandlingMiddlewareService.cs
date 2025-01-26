namespace WorkoutTracker.WebApi.Services.Interfaces
{
    public interface IExceptionHandlingMiddlewareService
    {
        Task WriteProblemDetailsResponseAsync(
            HttpContext context,
            Exception ex,
            int statusCode,
            string title);
    }
}
