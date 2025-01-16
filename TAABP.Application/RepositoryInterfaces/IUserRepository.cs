using Microsoft.AspNetCore.Identity;
using TAABP.Core;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IUserRepository
    {
        public Task<IdentityResult> CreateUserAsync(User user, string password);
        public Task<bool> CheckEmailAsync(string email);
        public Task<User> GetUserByEmailAsync(string email);
        public Task<User> GetUserByIdAsync(string id);
        public Task<List<User>> GetUsersAsync();
        public Task DeleteUserAsync(User user);
        public Task UpdateUserAsync(User user);
        public Task<List<Hotel>> GetLastHotelsVisitedAsync(string userId);
        Task<bool> CheckIfUserNameExists(string name);
    }
}
