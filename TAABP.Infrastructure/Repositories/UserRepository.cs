using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core;

namespace TAABP.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TAABPDbContext _context;
        private readonly UserManager<User> _userManager;
        public UserRepository(TAABPDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> CreateUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        public async Task<bool> CheckEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Hotel>> GetLastHotelsVisitedAsync(string userId)
        {
            var user = await _context.Users.AsNoTracking().Include(u => u.Reservations)
                .ThenInclude(r => r.Room)
                .ThenInclude(room => room.Hotel)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user!.Reservations
                .Select(r => r.Room.Hotel)
                .Where(h => h != null).Take(5)
                .ToList();
        }
    }
}
