using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.WebApi.Models.Responses;
using WorkoutTracker.WebApi.Models.WTUsers;
using WorkoutTracker.WebApi.Services.Interfaces;

namespace WorkoutTracker.WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class WTUsersController : ControllerBase
    {
        private readonly IWTUsersService _wtUsersService;
        private readonly ILogger<WTUsersController> _logger;
        private readonly IJwtService _jwtService;

        public WTUsersController(
            IWTUsersService wtUsersService,
            ILogger<WTUsersController> logger,
            IJwtService jwtService)
        {
            _wtUsersService = wtUsersService;
            _logger = logger;
            _jwtService = jwtService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterNewUser(CreateWTUserModel user)
        {
            _logger.LogInformation(
                "Received request to create a new user with username \"{Username}\"", 
                user.Username);

            var createdUser = await _wtUsersService.RegisterNewUserAsync(user);

            return CreatedAtAction(
                nameof(GetUserById),
                new { UserId = createdUser.Id },
                new ApiResponseModel<WTUserDto>
                {
                    Success = true,
                    Message = "User successfully created.",
                    Data = createdUser
                });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginWTUserModel user)
        {
            _logger.LogInformation("User {Username} is attempting to log in", user.Username);

            var jwt = await _wtUsersService.LogUserIn(user);

            return Ok(new ApiResponseModel<LoginResponseModel>
            {
                Success = true,
                Message = "Authentication successful.",
                Data = new LoginResponseModel { Token = jwt }
            });
        }

        [HttpGet("{userId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            _logger.LogInformation(
                "Received request to find a user with id \"{userId}\"",
                userId);

            var user = await _wtUsersService.GetUserByIdAsync(userId);

            return Ok(new ApiResponseModel<WTUserDto>
            {
                Success = true,
                Message = "User found.",
                Data = user
            });
        }

        [HttpGet("test-protected-route")]
        [Authorize]
        public IActionResult TestProtectedRoute()
        {
            return Ok("This is a protected endpoint!");
        }

        [HttpPost("problem-details")]
        public async Task<IActionResult> ProblemDetails()
        {
            await Task.CompletedTask;
            throw new KeyNotFoundException();
        }
    }
}
