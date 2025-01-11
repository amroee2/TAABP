using TAABP.Application.DTOs.ShoppingDto;
using TAABP.Core.ShoppingEntities;

namespace TAABP.Application.Profile.CartItemMapping
{
    public interface ICartItemMapper
    {
        void CartItemDtoToCartItem(CartItemDto cartItemDto, CartItem cartItem);
        CartItemDto CartItemToCartItemDto(CartItem cartItem);
    }
}
