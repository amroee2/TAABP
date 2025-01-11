using TAABP.Application.DTOs.ShoppingDto;
using TAABP.Core.ShoppingEntities;

namespace TAABP.Application.ServiceInterfaces
{
    public interface ICartItemService
    {
        Task<CartItem> GetCartItemByIdAsync(int cartItemId);
        Task<List<CartItem>> GetCartItemsByCartIdAsync(int cartId);
        Task<int> AddCartItemAsync(CartItemDto cartItem);
        Task DeleteCartItemAsync(int cartItemId);
    }
}
