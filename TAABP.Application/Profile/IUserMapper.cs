using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile
{
    public interface IUserMapper
    {
        User RegisterDtoToUser(RegisterDto registerDto);
        UserDto UserToUserDto(User user);
    }
}
