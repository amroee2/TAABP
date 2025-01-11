using Riok.Mapperly.Abstractions;
using TAABP.Application.DTOs.ShoppingDto;
using TAABP.Core.ShoppingEntities;

namespace TAABP.Application.Profile.CartItemMapping
{
    [Mapper]
    public partial class CartItemMapper : ICartItemMapper
    {
        public partial void CartItemDtoToCartItem(CartItemDto cartItemDto, CartItem cartItem);
        public partial CartItemDto CartItemToCartItemDto(CartItem cartItem);
    }
}
