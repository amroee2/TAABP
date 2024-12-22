using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.UserMapping
{
    public interface IUserMapper
    {
        User RegisterDtoToUser(RegisterDto registerDto);
        UserDto UserToUserDto(User user);
        void UserDtoToUser(UserDto userDto, User user);
    }
}
