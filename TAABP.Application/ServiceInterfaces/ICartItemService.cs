using TAABP.Application.DTOs.ShoppingDto;
using TAABP.Core.ShoppingEntities;

namespace TAABP.Application.ServiceInterfaces
{
    public interface ICartItemService
    {
        Task<CartItem> GetCartItemByIdAsync(int cartId, int cartItemId);
        Task<List<CartItem>> GetCartItemsByCartIdAsync(int cartId);
        Task<CartItemDto> AddCartItemAsync(string userId, CartItemDto cartItem);
        Task DeleteCartItemAsync(string userId, int cartItemId);
        Task<Cart> GetCartAsync(int cartId);
        Task ConfirmCartAsync(string userId, int paymentMethodId);
        Task<List<Cart>> GetUserCartsAsync(string userId);
    }
}
