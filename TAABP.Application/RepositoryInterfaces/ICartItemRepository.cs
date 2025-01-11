using TAABP.Core.ShoppingEntities;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface ICartItemRepository
    {
        Task<CartItem> GetCartItemByIdAsync(int cartId, int cartItemId);
        Task<List<CartItem>> GetCartItemsByCartIdAsync(int cartId);
        Task AddCartItemAsync(CartItem cartItem);
        Task DeleteCartItemAsync(CartItem cartItem);
    }
}
