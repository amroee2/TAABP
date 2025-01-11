using TAABP.Core.ShoppingEntities;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByIdAsync(int cartId);
        Task AddCartAsync(Cart cart);
        Task DeleteCartAsync(Cart cart);
        Task<List<Cart>> GetUserCarts(string userId);
        Task<bool> IsCartEmpty(int cartId);
        Task<bool> IsRoomAlreadyInCart(int cartId, int roomId);
    }
}
