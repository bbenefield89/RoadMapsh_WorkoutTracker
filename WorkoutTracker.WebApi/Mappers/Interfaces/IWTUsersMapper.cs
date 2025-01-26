using WorkoutTracker.WebApi.Data.Entities;
using WorkoutTracker.WebApi.Models.WTUsers;

namespace WorkoutTracker.WebApi.Mappers.Interfaces
{
    public interface IWTUsersMapper
    {
        WTUserEntity FromCreateModelToEntity(CreateWTUserModel createWTUserModel);

        WTUserDto FromEntityToDto(WTUserEntity userEntity);

        WTUserEntity FromLoginModelToEntity(CreateWTUserModel createWTUserModel);
    }
}
