using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.WebApi.Models.Responses;
using WorkoutTracker.WebApi.Services.Interfaces;

namespace WorkoutTracker.WebApi.Services
{
    public class ExceptionHandlingMiddlewareService : IExceptionHandlingMiddlewareService
    {
        private ILogger<ExceptionHandlingMiddlewareService> _logger;

        public ExceptionHandlingMiddlewareService(ILogger<ExceptionHandlingMiddlewareService> logger)
        {
            _logger = logger;
        }

        public async Task WriteProblemDetailsResponseAsync(
            HttpContext context,
            Exception ex,
            int statusCode,
            string title)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new ApiResponseModel<ProblemDetails>
            {
                Success = false,
                Message = ex.Message,
                Data = new ProblemDetails
                {
                    Title = title,
                    Detail = ex.Message,
                    Status = statusCode
                }
            });
        }
    }
}
