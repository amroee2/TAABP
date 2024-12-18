using Riok.Mapperly.Abstractions;
using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile
{
    [Mapper]
    public partial class UserMapper : IUserMapper
    {
        public partial User RegisterDtoToUser(RegisterDto registerDto);
    }
}
