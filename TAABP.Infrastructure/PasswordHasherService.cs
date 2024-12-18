using Microsoft.AspNetCore.Identity;
using TAABP.Application.PasswordHashing;

namespace TAABP.Infrastructure
{
    public class PasswordHasherService : IPasswordHasher
    {
        private readonly PasswordHasher<IdentityUser> _passwordHasher;

        public PasswordHasherService()
        {
            _passwordHasher = new PasswordHasher<IdentityUser>();
        }

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(new IdentityUser(), password);
        }
    }
}
