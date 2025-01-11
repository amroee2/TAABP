using TAABP.Application.DTOs.AccountDto;
using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IAccountService
    {
        public Task CreateUserAsync(RegisterDto registerDto);
        public Task<string> LoginAsync(LoginDto loginDto);
        public Task ChangeEmailAsync(string userId, ChangeEmailDto changeEmailDto);
        public Task ResetUserPasswordAsync(string email, string password);
    }
}
