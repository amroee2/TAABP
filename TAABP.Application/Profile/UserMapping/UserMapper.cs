using Riok.Mapperly.Abstractions;
using TAABP.Application.DTOs;
using TAABP.Application.Profile.UserMapping;
using TAABP.Core;

namespace TAABP.Application.Profile
{
    [Mapper]

    public partial class UserMapper : IUserMapper
    {
        public partial User RegisterDtoToUser(RegisterDto registerDto);
        public partial UserDto UserToUserDto(User user);
        public partial void UserDtoToUser(UserDto userDto, User user);
    }
}
