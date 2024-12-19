using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IUserService
    {
        public Task CreateUserAsync(RegisterDto registerDto);
        public Task<string> LoginAsync(LoginDto loginDto);
    }
}
