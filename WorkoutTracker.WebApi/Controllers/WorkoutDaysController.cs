using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.WebApi.Services.Interfaces;

namespace WorkoutTracker.WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class WorkoutDaysController : ControllerBase
    {
        private readonly IWorkoutDaysService _workoutDaysService;
        private readonly ILogger<WorkoutDaysController> _logger;

        public WorkoutDaysController(IWorkoutDaysService workoutDaysService, ILogger<WorkoutDaysController> logger)
        {
            _workoutDaysService = workoutDaysService;
            _logger = logger;
        }

        [HttpGet("date/{workoutDate:datetime}/user/{userId:guid}")]
        public async Task<IActionResult> GetWorkoutDayByDateAndUserId(DateOnly workoutDate, Guid userId)
        {
            _logger.LogInformation("Received request to fetch a WorkoutDay for User {UserId} on Date {WorkoutDate}",
                                    userId,
                                    workoutDate);

            try
            {
                var workoutDay = await _workoutDaysService.GetWorkoutDayByDateAndUserId(workoutDate, userId);

                if (workoutDay == null)
                {
                    _logger.LogWarning("Could not find a workout for user {UserId} on the date of {WorkoutDate}",
                                       userId,
                                       workoutDate);

                    return NotFound(new { Message = $"Could not find a workout for user {userId} on the date of {workoutDate}" });
                }

                return Ok(workoutDay);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                                 "Error trying to fetch WorkoutDay for User {UserId} on Date {WorkoutDate}",
                                 userId,
                                 workoutDate);

                return StatusCode(500, "Something went wrong");
            }
        }

        [HttpGet("test")]
        public async Task<IActionResult> GetWorkoutDayByDateAndUserId()
        {
            var workoutDate = DateOnly.Parse("2025-01-10");
            var userId = Guid.Parse("4615f7bd-c352-4df1-a28a-746c446eeb38");

            var workoutDay = await _workoutDaysService.GetWorkoutDayByDateAndUserId(workoutDate, userId);

            return Ok(workoutDay);
        }
    }
}
