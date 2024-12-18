using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TAABP.Core;

namespace TAABP.Infrastructure
{
    public class TAABPDbContext : IdentityDbContext<User>
    {
        public TAABPDbContext(DbContextOptions<TAABPDbContext> options) : base(options)
        {
        }

    }
}
