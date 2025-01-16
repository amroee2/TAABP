using TAABP.Application.DTOs;
using TAABP.Application.DTOs.ShoppingDto;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.CartItemMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core;
using TAABP.Core.ShoppingEntities;

namespace TAABP.Application.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemMapper _cartItemMapper;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IReservationService _reservationService;
        public CartItemService(ICartItemRepository cartItemRepository, ICartRepository cartRepository,
            IRoomRepository roomRepository, ICartItemMapper cartItemMapper,
            IPaymentMethodRepository pwaymentMethodRepository, IReservationService reservationService)
        {
            _cartItemRepository = cartItemRepository;
            _cartRepository = cartRepository;
            _roomRepository = roomRepository;
            _cartItemMapper = cartItemMapper;
            _paymentMethodRepository = pwaymentMethodRepository;
            _reservationService = reservationService;
        }

        public async Task<CartItem> GetCartItemByIdAsync(int cartId, int cartItemId)
        {
            var cartItem = await _cartItemRepository.GetCartItemByIdAsync(cartId, cartItemId);
            if (cartItem == null)
            {
                throw new EntityNotFoundException("CartItem not found");
            }
            return cartItem;
        }

        public async Task<List<CartItem>> GetCartItemsByCartIdAsync(int cartId)
        {
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            if (cart == null)
            {
                throw new EntityNotFoundException("Cart not found");
            }
            var cartItems = await _cartItemRepository.GetCartItemsByCartIdAsync(cartId);
            return cartItems;
        }

        public async Task<CartItemDto> AddCartItemAsync(string userId, CartItemDto cartItemDto)
        {
            var cartItem = new CartItem();
            _cartItemMapper.CartItemDtoToCartItem(cartItemDto, cartItem);
            var cart = await _cartRepository.GetUserRecentCartAsync(userId);
            if (cart == null)
            {
                cart = new Cart();
                cart.UserId = userId;
                cart.CartStatus = CartStatus.Open;
                await _cartRepository.AddCartAsync(cart);
                cartItemDto.CartId = cart.CartId;
            }
            if (cart.CartItems.Contains(cartItem))
            {
                throw new EntityCreationException("CartItem already exists in Cart");
            }
            var room = await _roomRepository.GetRoomByIdAsync(cartItemDto.RoomId);
            if (room == null)
            {
                throw new EntityNotFoundException("Room not found");
            }
            if (room.IsAvailable == false)
            {
                throw new RoomAlreadyBookedException("Room is not available");
            }
            if (await _cartRepository.IsRoomAlreadyInCart(cart.CartId, room.RoomId))
            {
                throw new EntityCreationException("Room is already in cart");
            }

            cartItem.Price = await _reservationService.CalculateTotoalPriceAsync(room, cartItem.StartDate, cartItem.EndDate);
            cartItem.CartId = cart.CartId; 
            await _cartRepository.AddToTotalPriceAsync(cartItem.Price, cart.CartId);
            await _cartItemRepository.AddCartItemAsync(cartItem);
            return _cartItemMapper.CartItemToCartItemDto(cartItem);
        }

        public async Task DeleteCartItemAsync(string userId, int cartItemId)
        {
            var cart = await _cartRepository.GetUserRecentCartAsync(userId);
            if (cart == null)
            {
                throw new EntityNotFoundException("Cart not found");
            }
            var cartItem = await _cartItemRepository.GetCartItemByIdAsync(cart.CartId, cartItemId);
            if (cartItem == null)
            {
                throw new EntityNotFoundException("CartItem not found");
            }

            if (cart.CartStatus == CartStatus.Closed)
            {
                throw new EntityCreationException("Cart is already closed");
            }
            await _cartRepository.RemoveFromTotalPriceAsync(cartItem.Price, cart.CartId);
            await _cartItemRepository.DeleteCartItemAsync(cartItem);
            if (await _cartRepository.IsCartEmpty(cart.CartId))
            {
                await _cartRepository.DeleteCartAsync(cart);
            }
        }

        public async Task<Cart> GetCartAsync(int cartId)
        {
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            if (cart == null)
            {
                throw new EntityNotFoundException("Cart not found");
            }
            return cart;
        }

        public async Task ConfirmCartAsync(string userId, int paymentMethodId)
        {
            var cart = await _cartRepository.GetUserRecentCartAsync(userId);
            if (cart == null)
            {
                throw new EntityNotFoundException("Cart not found");
            }
            if (cart.CartStatus == CartStatus.Closed)
            {
                throw new EntityCreationException("Cart is already closed");
            }
            if (!cart.CartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty");
            }
            var paymentMethod = await _paymentMethodRepository.GetPaymentMethodByIdAsync(paymentMethodId);
            if (paymentMethod == null)
            {
                throw new EntityNotFoundException("PaymentMethod not found");
            }
            if(paymentMethod.UserId != userId)
            {
                throw new EntityCreationException("PaymentMethod does not belong to user");
            }
            //process payment
            await _cartRepository.UpdateCartStatusAsync(cart.CartId, CartStatus.Closed);
            foreach (var item in cart.CartItems)
            {
                var reservationDto = new ReservationDto();
                reservationDto.RoomId = item.RoomId;
                reservationDto.UserId = userId;
                reservationDto.StartDate = item.StartDate;
                reservationDto.EndDate = item.EndDate;
                await _reservationService.CreateReservationAsync(reservationDto);
            }
        }

        public async Task<List<Cart>> GetUserCartsAsync(string userId)
        {
            var carts = await _cartRepository.GetUserCartsAsync(userId);
            return carts;
        }
    }
}
