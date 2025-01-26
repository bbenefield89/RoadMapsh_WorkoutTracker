using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WorkoutTracker.WebApi.Controllers;
using WorkoutTracker.WebApi.Models.WorkoutDays;
using WorkoutTracker.WebApi.Services.Interfaces;

namespace WorkoutTracker.Tests.Controllers
{
    public class WorkoutDaysControllerTests
    {
        private readonly Mock<IWorkoutDaysService> _mockWorkoutDaysService;
        private readonly Mock<ILogger<WorkoutDaysController>> _mockLogger;
        private readonly WorkoutDaysController _controller;

        public WorkoutDaysControllerTests()
        {
            _mockWorkoutDaysService = new Mock<IWorkoutDaysService>();
            _mockLogger = new Mock<ILogger<WorkoutDaysController>>();
            _controller = new WorkoutDaysController(_mockWorkoutDaysService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetWorkoutDayByDateAndUserId_ReturnsOkWithWorkoutDay_WhenWorkoutDayExists()
        {
            // Arrange
            var workoutDate = new DateOnly(2025, 1, 10);
            var userId = Guid.NewGuid();
            var expectedWorkoutDay = new WorkoutDayDto
            {
                Id = Guid.NewGuid(),
                WorkoutDate = workoutDate,
                WorkoutDayExercises = null // Simplify for this test
            };

            _mockWorkoutDaysService
                .Setup(s => s.GetWorkoutDayByDateAndUserId(workoutDate, userId))
                .ReturnsAsync(expectedWorkoutDay);

            // Act
            var result = await _controller.GetWorkoutDayByDateAndUserId(workoutDate, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedWorkoutDay, okResult.Value);
        }

        [Fact]
        public async Task GetWorkoutDayByDateAndUserId_ReturnsNotFound_WhenWorkoutDayDoesNotExist()
        {
            // Arrange
            var workoutDate = new DateOnly(2025, 1, 10);
            var userId = Guid.NewGuid();

            _mockWorkoutDaysService
                .Setup(s => s.GetWorkoutDayByDateAndUserId(workoutDate, userId))
                .ReturnsAsync((WorkoutDayDto?)null);

            // Act
            var result = await _controller.GetWorkoutDayByDateAndUserId(workoutDate, userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains(userId.ToString(), notFoundResult.Value.ToString());
            Assert.Contains(workoutDate.ToString(), notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task GetWorkoutDayByDateAndUserId_HandlesException_Returns500()
        {
            // Arrange
            var workoutDate = new DateOnly(2025, 1, 10);
            var userId = Guid.NewGuid();

            _mockWorkoutDaysService
                .Setup(s => s.GetWorkoutDayByDateAndUserId(workoutDate, userId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetWorkoutDayByDateAndUserId(workoutDate, userId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Something went wrong", objectResult.Value);
        }
    }
}
