using Microsoft.EntityFrameworkCore;
using TAABP.Core.ShoppingEntities;

namespace TAABP.Infrastructure.Repositories.ShoppingRepositories
{
    public class CartRepository
    {
        private readonly TAABPDbContext _context;

        public CartRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public async Task AddCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async Task<Cart> GetCartByIdAsync(int cartId)
        {
            return await _context.Carts.AsNoTracking().FirstOrDefaultAsync(c => c.CartId == cartId);
        }

        public async Task GetUserCarts(string userId)
        {
            var userCarts = await _context.Carts
                .Include(c => c.PaymentMethod)
                .ThenInclude(pm => pm.User)
                .Where(c => c.PaymentMethod.UserId == userId)
                .ToListAsync();
        }

        public async Task DeleteCartAsync(Cart cart)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }
    }
}
