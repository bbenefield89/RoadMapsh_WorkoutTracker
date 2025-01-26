using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.WebApi.Exceptions;
using WorkoutTracker.WebApi.Models.Responses;
using WorkoutTracker.WebApi.Models.WTUsers;
using WorkoutTracker.WebApi.Services.Interfaces;

namespace WorkoutTracker.WebApi.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionHandlingMiddlewareService _exceptionHandlingMiddlewareService;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IExceptionHandlingMiddlewareService exceptionHandlingMiddlewareService)
        {
            _next = next;
            _logger = logger;
            _exceptionHandlingMiddlewareService = exceptionHandlingMiddlewareService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var (statusCode, title) = ex switch
                {
                    UserAlreadyExistsException => (StatusCodes.Status400BadRequest, "User Already Exists"),
                    UserNotFoundException => (StatusCodes.Status404NotFound, "User Not Found"),
                    InvalidLoginCredentialsException => (StatusCodes.Status400BadRequest, "Invalid Login Credentials"),
                    KeyNotFoundException => (StatusCodes.Status404NotFound, "Entity Not Found"),
                    _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
                };

                await _exceptionHandlingMiddlewareService.WriteProblemDetailsResponseAsync(
                    context,
                    ex,
                    statusCode,
                    title);
            }
        }

    }
}
