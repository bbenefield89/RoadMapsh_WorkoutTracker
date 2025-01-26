using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using WorkoutTracker.WebApi.Data.Entities;
using WorkoutTracker.WebApi.Exceptions;
using WorkoutTracker.WebApi.Mappers.Interfaces;
using WorkoutTracker.WebApi.Models.WTUsers;
using WorkoutTracker.WebApi.Repositories.Interfaces;
using WorkoutTracker.WebApi.Services.Interfaces;

namespace WorkoutTracker.WebApi.Services
{
    public class WTUsersService : IWTUsersService
    {
        private IWTUsersRepository _wtUsersRepository;
        private IPasswordHasher<WTUserEntity> _passwordHasher;
        private IWTUsersMapper _usersMapper;
        private IJwtService _jwtService;
        private ILogger<WTUsersService> _logger;

        public WTUsersService(
            IWTUsersRepository wtUsersRepository,
            IPasswordHasher<WTUserEntity> passwordHasher,
            IWTUsersMapper usersMapper,
            ILogger<WTUsersService> logger,
            IJwtService jwtService)
        {
            _wtUsersRepository = wtUsersRepository;
            _passwordHasher = passwordHasher;
            _usersMapper = usersMapper;
            _logger = logger;
            _jwtService = jwtService;
        }

        public async Task<WTUserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _wtUsersRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            _logger.LogInformation("Successfully found user by Id '{userId}'", userId);

            return _usersMapper.FromEntityToDto(user);
        }

        public async Task<WTUserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _wtUsersRepository.GetWTUserByUsernameAsync(username);

            if (user == null)
            {
                throw new UserNotFoundException(username);
            }

            return _usersMapper.FromEntityToDto(user);
        }

        public async Task<WTUserDto> RegisterNewUserAsync(CreateWTUserModel user)
        {
            var doesUserExist = await DoesUserExistByUsernameAsync(user.Username);

            if (doesUserExist)
            {
                throw new UserAlreadyExistsException(user.Username);
            }

            var createdUser = CreateWTUser(user);

            _logger.LogInformation("New user with username \"{Username}\" successfully created",
                       user.Username);

            return createdUser;
        }

        public async Task<bool> DoesUserExistByUsernameAsync(string username)
        {
            Expression<Func<WTUserEntity, bool>> predicate = i => i.Username == username;

            var doesUserExist = await _wtUsersRepository.DoesEntityExistByConditionAsync(predicate);

            return doesUserExist;
        }

        public WTUserDto CreateWTUser(CreateWTUserModel createUserModel)
        {
            var userEntity = _usersMapper.FromCreateModelToEntity(createUserModel);

            userEntity.Password = _passwordHasher.HashPassword(userEntity, createUserModel.Password);

            var createdUser = _wtUsersRepository.CreateWTUser(userEntity);

            return _usersMapper.FromEntityToDto(createdUser);
        }

        public async Task<string> LogUserIn(LoginWTUserModel user)
        {
            var doPasswordsMatch = await _jwtService.VerifyHashedPassword(
                user,
                user.Password);

            if (!doPasswordsMatch)
            {
                _logger.LogWarning("Authentication failed for user {Username}. Invalid credentials",
                                   user.Username);

                throw new InvalidLoginCredentialsException();
            }

            var token = _jwtService.GenerateJwtToken(user.Username);

            _logger.LogInformation(
                "User {Username} has successfully authenticated",
                user.Username);

            return token;
        }
    }
}
