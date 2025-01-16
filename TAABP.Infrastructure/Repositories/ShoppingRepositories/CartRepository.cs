using Microsoft.EntityFrameworkCore;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core;
using TAABP.Core.ShoppingEntities;

namespace TAABP.Infrastructure.Repositories.ShoppingRepositories
{
    public class CartRepository : ICartRepository
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

        public async Task<Cart> GetCartByIdAsync( int cartId)
        {
            var cart = await _context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            if (cart != null)
            {
                _context.Entry(cart).Collection(c => c.CartItems).Query().AsNoTracking();
            }

            return cart;
        }

        public async Task<List<Cart>> GetUserCartsAsync(string userId)
        {
            var userCarts = await _context.Carts
                .Include(pm => pm.User)
                .Where(c=>c.UserId==userId)
                .ToListAsync();
            return userCarts;
        }

        public async Task DeleteCartAsync(Cart cart)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsCartEmpty(int cartId)
        {
            return !await _context.CartItems
                .AnyAsync(ci => ci.CartId == cartId);
        }

        public async Task<bool> IsRoomAlreadyInCart(int cartId, int roomId)
        {
            return await _context.CartItems
                .AnyAsync(ci => ci.CartId == cartId && ci.RoomId == roomId);
        }

        public async Task<Cart> GetUserRecentCartAsync(string userId)
        {
            return await _context.Carts.AsNoTracking()
                .Include(c => c.User)
                .Include(c => c.CartItems)
                .Where(c => c.UserId == userId)
                .Where(c => c.CartStatus == CartStatus.Open)
                .OrderByDescending(c => c.CartId)
                .FirstOrDefaultAsync();
        }

        public async Task AddToTotalPriceAsync(double price, int cartId)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            cart.TotalPrice+= price;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromTotalPriceAsync(double price, int cartId)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            cart.TotalPrice -= price;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartStatusAsync(int cartId, CartStatus cartStatus)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            cart.CartStatus = cartStatus;
            await _context.SaveChangesAsync();
        }
    }
}
