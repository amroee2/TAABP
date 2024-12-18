using TAABP.Core;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IUserService
    {
        public Task CreateUserAsync(User user);
    }
}
