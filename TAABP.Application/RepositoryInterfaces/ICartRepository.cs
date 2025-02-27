﻿using TAABP.Core;
using TAABP.Core.ShoppingEntities;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByIdAsync( int cartId);
        Task AddCartAsync(Cart cart);
        Task DeleteCartAsync(Cart cart);
        Task<List<Cart>> GetUserCartsAsync(string userId);
        Task<bool> IsCartEmpty(int cartId);
        Task<bool> IsRoomAlreadyInCart(int cartId, int roomId);
        Task<Cart> GetUserRecentCartAsync(string userId);
        Task AddToTotalPriceAsync(double price, int cartId);
        Task RemoveFromTotalPriceAsync(double price, int cartId);
        Task UpdateCartStatusAsync(int cartId, CartStatus cartStatus);

    }
}
