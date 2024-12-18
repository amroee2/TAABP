using TAABP.Core;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IUserRepository
    {
        public Task CreateUserAsync(User user);
    }
}
