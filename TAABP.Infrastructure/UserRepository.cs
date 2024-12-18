﻿using TAABP.Application.RepositoryInterfaces;
using TAABP.Core;

namespace TAABP.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly TAABPDbContext _context;

        public UserRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public async Task CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
