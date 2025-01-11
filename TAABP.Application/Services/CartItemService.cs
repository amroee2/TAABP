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
        private readonly IPaymentMethodRepository _pwaymentMethodRepository;
        private readonly IReservationService _reservationService;
        public CartItemService(ICartItemRepository cartItemRepository, ICartRepository cartRepository,
            IRoomRepository roomRepository, ICartItemMapper cartItemMapper,
            IPaymentMethodRepository pwaymentMethodRepository, IReservationService reservationService)
        {
            _cartItemRepository = cartItemRepository;
            _cartRepository = cartRepository;
            _roomRepository = roomRepository;
            _cartItemMapper = cartItemMapper;
            _pwaymentMethodRepository = pwaymentMethodRepository;
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

        public async Task<CartItemDto> AddCartItemAsync(CartItemDto cartItemDto)
        {
            var cartItem = new CartItem();
            _cartItemMapper.CartItemDtoToCartItem(cartItemDto, cartItem);
            var cart = await _cartRepository.GetCartByIdAsync(cartItem.CartId);
            if (cart == null)
            {
                cart = new Cart();
                cart.CartStatus = CartStatus.Open;
                await _cartRepository.AddCartAsync(cart);
                cartItem.CartId = cart.CartId;
            }

            else if (cart.CartStatus == CartStatus.Closed)
            {
                throw new EntityCreationException("Cart is already closed");
            }

            else if (cart.CartItems.Contains(cartItem))
            {
                throw new EntityCreationException("CartItem already exists in Cart");
            }
            var room = await _roomRepository.GetRoomByIdAsync(cartItem.RoomId);
            if (room == null)
            {
                throw new EntityNotFoundException("Room not found");
            }
            if (room.IsAvailable == false)
            {
                throw new RoomAlreadyBookedException("Room is not available");
            }
            if (await _cartRepository.IsRoomAlreadyInCart(cartItem.CartId, cartItem.RoomId))
            {
                throw new EntityCreationException("Room already in cart");
            }
            cartItem.Price = (cartItem.EndDate - cartItem.StartDate).Days * room.PricePerNight;
            if (cartItem.Price == 0)
            {
                cartItem.Price = room.PricePerNight;
            }
            cart.CartItems.Add(cartItem);
            cart.AddToPrice(cartItem.Price);
            await _cartItemRepository.AddCartItemAsync(cartItem);
            return _cartItemMapper.CartItemToCartItemDto(cartItem);
        }

        public async Task DeleteCartItemAsync(int cartId, int cartItemId)
        {
            var cartItem = await _cartItemRepository.GetCartItemByIdAsync(cartId, cartItemId);
            if (cartItem == null)
            {
                throw new EntityNotFoundException("CartItem not found");
            }
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            if (cart == null)
            {
                throw new EntityNotFoundException("Cart not found");
            }
            if (cart.CartStatus == CartStatus.Closed)
            {
                throw new EntityCreationException("Cart is already closed");
            }
            cart.RemoveFromPrice(cartItem.Price);
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

        public async Task ConfirmCartAsync(int cartId, int paymentMethodId)
        {
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            if (cart == null)
            {
                throw new EntityNotFoundException("Cart not found");
            }
            if (cart.CartStatus == CartStatus.Closed)
            {
                throw new EntityCreationException("Cart is already closed");
            }
            var payment = await _pwaymentMethodRepository.GetPaymentMethodByIdAsync(paymentMethodId);
            if (payment == null)
            {
                throw new EntityNotFoundException("Payment method not found");
            }
            cart.PaymentMethodId = payment.PaymentMethodId;
            cart.CartStatus = CartStatus.Closed;
            var user = await _pwaymentMethodRepository.GetUserByPaymentMethodId(paymentMethodId);
            foreach(var item in cart.CartItems)
            {
                var reservationDto = new ReservationDto();
                reservationDto.RoomId = item.RoomId;
                reservationDto.UserId = user.Id;
                reservationDto.StartDate = item.StartDate;
                reservationDto.EndDate = item.EndDate;
                await _reservationService.CreateReservationAsync(reservationDto);
            }
        }
    }
}
