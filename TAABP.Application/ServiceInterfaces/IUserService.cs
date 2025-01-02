using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IUserService
    {
        public Task CreateUserAsync(RegisterDto registerDto);
        public Task<string> LoginAsync(LoginDto loginDto);
        public Task<UserDto> GetUserByIdAsync(string id);
        public Task<List<UserDto>> GetUsersAsync();
        public Task DeleteUserAsync(string id);
        public Task UpdateUserAsync(string Id, UserDto userDto);
        public Task<string> GetCurrentUsernameAsync();
        public string GetCurrentUserId();
        public Task<List<HotelDto>> GetLastHotelsVisitedAsync(string userId);
        public Task<bool> CheckEmailAsync(string email);
    }
}
