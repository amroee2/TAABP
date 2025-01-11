using Microsoft.EntityFrameworkCore;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core.ShoppingEntities;

namespace TAABP.Infrastructure.Repositories.ShoppingRepositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly TAABPDbContext _context;

        public CartItemRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public async Task AddCartItemAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task<CartItem> GetCartItemByIdAsync(int cartItemId)
        {
            return await _context.CartItems.AsNoTracking().FirstOrDefaultAsync(c=> c.CartItemId == cartItemId);
        }

        public async Task<List<CartItem>> GetCartItemsByCartIdAsync(int cartId)
        {
            return await _context.CartItems.AsNoTracking().Where(c => c.CartId == cartId).ToListAsync();
        }

        public async Task DeleteCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }
    }
}
