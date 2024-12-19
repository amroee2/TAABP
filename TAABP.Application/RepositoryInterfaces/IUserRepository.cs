using TAABP.Core;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IUserRepository
    {
        public Task CreateUserAsync(User user);
        public Task<bool> CheckEmailAsync(string email);
        public Task<User> GetUserByEmailAsync(string email);
    }
}
