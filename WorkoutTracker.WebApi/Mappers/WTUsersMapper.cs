using WorkoutTracker.WebApi.Data.Entities;
using WorkoutTracker.WebApi.Mappers.Interfaces;
using WorkoutTracker.WebApi.Models.WTUsers;

namespace WorkoutTracker.WebApi.Mappers
{
    public class WTUsersMapper : IWTUsersMapper
    {
        public WTUserDto FromEntityToDto(WTUserEntity userEntity)
        {
            return new WTUserDto
            {
                Id = userEntity.Id,
                Username = userEntity.Username
            };
        }

        public WTUserEntity FromCreateModelToEntity(CreateWTUserModel createWTUserModel)
        {
            return new WTUserEntity
            {
                Username = createWTUserModel.Username,
                Password = createWTUserModel.Password,
            };
        }

        public WTUserEntity FromLoginModelToEntity(CreateWTUserModel createWTUserModel)
        {
            return new WTUserEntity
            {
                Username = createWTUserModel.Username,
                Password = createWTUserModel.Password,
            };
        }
    }
}
