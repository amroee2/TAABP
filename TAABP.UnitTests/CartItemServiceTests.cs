using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using TAABP.Application.DTOs;
using TAABP.Application.DTOs.ShoppingDto;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.CartItemMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.Services;
using TAABP.Core;
using TAABP.Core.PaymentEntities;
using TAABP.Core.ShoppingEntities;

namespace TAABP.UnitTests
{
    public class CartItemServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ICartItemRepository> _cartItemRepositoryMock;
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Mock<IRoomRepository> _roomRepositoryMock;
        private readonly Mock<ICartItemMapper> _cartItemMapperMock;
        private readonly Mock<IPaymentMethodRepository> _paymentMethodRepositoryMock;
        private readonly Mock<IReservationService> _reservationServiceMock;
        private readonly CartItemService _cartItemService;
        public CartItemServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _cartItemRepositoryMock = _fixture.Freeze<Mock<ICartItemRepository>>();
            _cartRepositoryMock = _fixture.Freeze<Mock<ICartRepository>>();
            _roomRepositoryMock = _fixture.Freeze<Mock<IRoomRepository>>();
            _cartItemMapperMock = _fixture.Freeze<Mock<ICartItemMapper>>();
            _paymentMethodRepositoryMock = _fixture.Freeze<Mock<IPaymentMethodRepository>>();
            _reservationServiceMock = _fixture.Freeze<Mock<IReservationService>>();

            _cartItemService = new CartItemService(
                _cartItemRepositoryMock.Object,
                _cartRepositoryMock.Object,
                _roomRepositoryMock.Object,
                _cartItemMapperMock.Object,
                _paymentMethodRepositoryMock.Object,
                _reservationServiceMock.Object);
        }

        [Fact]
        public async Task GetCartItemByIdAsync_ValidId_ReturnsCartItem()
        {
            // Arrange
            var cartId = _fixture.Create<int>();
            var cartItemId = _fixture.Create<int>();
            var cartItem = _fixture.Create<CartItem>();

            _cartItemRepositoryMock
                .Setup(repo => repo.GetCartItemByIdAsync(cartId, cartItemId))
                .ReturnsAsync(cartItem);

            // Act
            var result = await _cartItemService.GetCartItemByIdAsync(cartId, cartItemId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartItem, result);
            _cartItemRepositoryMock.Verify(repo => repo.GetCartItemByIdAsync(cartId, cartItemId), Times.Once);
        }

        [Fact]
        public async Task GetCartItemByIdAsync_InvalidId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var cartId = _fixture.Create<int>();
            var cartItemId = _fixture.Create<int>();

            _cartItemRepositoryMock
                .Setup(repo => repo.GetCartItemByIdAsync(cartId, cartItemId))
                .ReturnsAsync((CartItem)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _cartItemService.GetCartItemByIdAsync(cartId, cartItemId));
            _cartItemRepositoryMock.Verify(repo => repo.GetCartItemByIdAsync(cartId, cartItemId), Times.Once);
        }

        [Fact]
        public async Task GetCartItemsByCartIdAsync_ValidId_ReturnsCartItems()
        {
            // Arrange
            var cartId = _fixture.Create<int>();
            var cart = _fixture.Create<Cart>();
            var cartItems = _fixture.CreateMany<CartItem>().ToList();

            _cartRepositoryMock
                .Setup(repo => repo.GetCartByIdAsync(cartId))
                .ReturnsAsync(cart);

            _cartItemRepositoryMock
                .Setup(repo => repo.GetCartItemsByCartIdAsync(cartId))
                .ReturnsAsync(cartItems);

            // Act
            var result = await _cartItemService.GetCartItemsByCartIdAsync(cartId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartItems, result);
            _cartRepositoryMock.Verify(repo => repo.GetCartByIdAsync(cartId), Times.Once);
            _cartItemRepositoryMock.Verify(repo => repo.GetCartItemsByCartIdAsync(cartId), Times.Once);
        }

        [Fact]
        public async Task GetCartItemsByCartIdAsync_InvalidId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var cartId = _fixture.Create<int>();

            _cartRepositoryMock
                .Setup(repo => repo.GetCartByIdAsync(cartId))
                .ReturnsAsync((Cart)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _cartItemService.GetCartItemsByCartIdAsync(cartId));
            _cartRepositoryMock.Verify(repo => repo.GetCartByIdAsync(cartId), Times.Once);
        }

        [Fact]
        public async Task AddCartItemAsync_ValidDto_ReturnsCartItemDto()
        {
            // Arrange
            var cartItemDto = _fixture.Create<CartItemDto>();

            var room = _fixture.Build<Room>().With(r => r.IsAvailable, true).Create();
            var cartItem = _fixture.Build<CartItem>()
                .With(ci => ci.Room, room)
                .With(ci => ci.CartId, cartItemDto.CartId)
                .With(ci => ci.RoomId, cartItemDto.RoomId)
                .With(ci => ci.StartDate, cartItemDto.StartDate)
                .With(ci => ci.EndDate, cartItemDto.EndDate)
                .Create();

            var cart = _fixture.Build<Cart>()
                .With(c => c.CartStatus, CartStatus.Open)
                .With(c => c.CartItems, new List<CartItem>())
                .Create();

            _cartRepositoryMock
                .Setup(repo => repo.GetCartByIdAsync(cartItem.CartId))
                .ReturnsAsync(cart);

            _cartRepositoryMock
                .Setup(repo => repo.AddCartAsync(It.IsAny<Cart>()))
                .Callback<Cart>(c => c.CartId = cart.CartId);

            _cartRepositoryMock
                .Setup(repo => repo.IsRoomAlreadyInCart(cartItem.CartId, cartItem.RoomId))
                .ReturnsAsync(false);

            _cartItemMapperMock
                .Setup(mapper => mapper.CartItemDtoToCartItem(cartItemDto, It.IsAny<CartItem>()))
                .Callback((CartItemDto dto, CartItem ci) =>
                {
                    ci.CartId = dto.CartId;
                    ci.RoomId = dto.RoomId;
                    ci.StartDate = dto.StartDate;
                    ci.EndDate = dto.EndDate;
                });

            _cartItemMapperMock
                .Setup(mapper => mapper.CartItemToCartItemDto(It.IsAny<CartItem>()))
                .Returns(cartItemDto);

            _cartItemRepositoryMock
                .Setup(repo => repo.AddCartItemAsync(It.IsAny<CartItem>()));

            _roomRepositoryMock
                .Setup(repo => repo.GetRoomByIdAsync(cartItem.RoomId))
                .ReturnsAsync(room);

            // Act
            var result = await _cartItemService.AddCartItemAsync(cartItemDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartItemDto.CartId, result.CartId);
            Assert.Equal(cartItemDto.RoomId, result.RoomId);
            Assert.Equal(cartItemDto.StartDate, result.StartDate);
            Assert.Equal(cartItemDto.EndDate, result.EndDate);

            _cartRepositoryMock.Verify(repo => repo.GetCartByIdAsync(cartItem.CartId), Times.Once);
            _cartRepositoryMock.Verify(repo => repo.IsRoomAlreadyInCart(cartItem.CartId, cartItem.RoomId), Times.Once);
            _cartItemMapperMock.Verify(mapper => mapper.CartItemDtoToCartItem(cartItemDto, It.IsAny<CartItem>()), Times.Once);
            _cartItemRepositoryMock.Verify(repo => repo.AddCartItemAsync(It.IsAny<CartItem>()), Times.Once);
            _roomRepositoryMock.Verify(repo => repo.GetRoomByIdAsync(cartItem.RoomId), Times.Once);
        }

        [Fact]
        public async Task DeleteCartItemAsync_ShouldDeleteCartItemAsync()
        {
            // Arrange
            var cartId = _fixture.Create<int>();
            var cartItemId = _fixture.Create<int>();
            var cartItem = _fixture.Create<CartItem>();

            _cartItemRepositoryMock
                .Setup(repo => repo.GetCartItemByIdAsync(cartId, cartItemId))
                .ReturnsAsync(cartItem);

            _cartItemRepositoryMock
                .Setup(repo => repo.DeleteCartItemAsync(cartItem));

            // Act
            await _cartItemService.DeleteCartItemAsync(cartId, cartItemId);

            // Assert
            _cartItemRepositoryMock.Verify(repo => repo.GetCartItemByIdAsync(cartId, cartItemId), Times.Once);
            _cartItemRepositoryMock.Verify(repo => repo.DeleteCartItemAsync(cartItem), Times.Once);
        }

        [Fact]
        public async Task AddCartItemAsync_CartIsClosed_ThrowsEntityCreationException()
        {
            // Arrange
            var cartItemDto = _fixture.Create<CartItemDto>();

            var closedCart = _fixture.Build<Cart>()
                .With(c => c.CartStatus, CartStatus.Closed)
                .Create();

            _cartRepositoryMock
                .Setup(repo => repo.GetCartByIdAsync(cartItemDto.CartId))
                .ReturnsAsync(closedCart);

            _cartItemMapperMock
                .Setup(mapper => mapper.CartItemDtoToCartItem(cartItemDto, It.IsAny<CartItem>()))
                .Callback((CartItemDto dto, CartItem ci) =>
                {
                    ci.CartId = dto.CartId;
                    ci.RoomId = dto.RoomId;
                    ci.StartDate = dto.StartDate;
                    ci.EndDate = dto.EndDate;
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityCreationException>(
                async () => await _cartItemService.AddCartItemAsync(cartItemDto));

            Assert.Equal("Cart is already closed", exception.Message);

            _cartRepositoryMock.Verify(repo => repo.GetCartByIdAsync(cartItemDto.CartId), Times.Once);
            _cartItemRepositoryMock.Verify(repo => repo.AddCartItemAsync(It.IsAny<CartItem>()), Times.Never);
            _roomRepositoryMock.Verify(repo => repo.GetRoomByIdAsync(_fixture.Create<int>()), Times.Never);
        }

        [Fact]
        public async Task AddCartItemAsync_RoomNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            var cartItemDto = _fixture.Create<CartItemDto>();

            var cart = _fixture.Build<Cart>()
                .With(c => c.CartStatus, CartStatus.Open)
                .With(c => c.CartItems, new List<CartItem>())
                .Create();

            _cartRepositoryMock
                .Setup(repo => repo.GetCartByIdAsync(cartItemDto.CartId))
                .ReturnsAsync(cart);

            _roomRepositoryMock
                .Setup(repo => repo.GetRoomByIdAsync(cartItemDto.RoomId))
                .ReturnsAsync((Room)null); 

            _cartItemMapperMock
                .Setup(mapper => mapper.CartItemDtoToCartItem(cartItemDto, It.IsAny<CartItem>()))
                .Callback((CartItemDto dto, CartItem ci) =>
                {
                    ci.CartId = dto.CartId;
                    ci.RoomId = dto.RoomId;
                    ci.StartDate = dto.StartDate;
                    ci.EndDate = dto.EndDate;
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
                async () => await _cartItemService.AddCartItemAsync(cartItemDto));

            Assert.Equal("Room not found", exception.Message);

            _cartRepositoryMock.Verify(repo => repo.GetCartByIdAsync(cartItemDto.CartId), Times.Once);
            _roomRepositoryMock.Verify(repo => repo.GetRoomByIdAsync(cartItemDto.RoomId), Times.Once);
            _cartItemRepositoryMock.Verify(repo => repo.AddCartItemAsync(It.IsAny<CartItem>()), Times.Never);
        }

        [Fact]
        public async Task AddCartItemAsync_RoomNotAvailable_ThrowsRoomAlreadyBookedException()
        {
            // Arrange
            var cartItemDto = _fixture.Create<CartItemDto>();

            var cart = _fixture.Build<Cart>()
                .With(c => c.CartStatus, CartStatus.Open)
                .With(c => c.CartItems, new List<CartItem>())
                .Create();

            var unavailableRoom = _fixture.Build<Room>()
                .With(r => r.IsAvailable, false)
                .Create();

            _cartRepositoryMock
                .Setup(repo => repo.GetCartByIdAsync(cartItemDto.CartId))
                .ReturnsAsync(cart);

            _roomRepositoryMock
                .Setup(repo => repo.GetRoomByIdAsync(cartItemDto.RoomId))
                .ReturnsAsync(unavailableRoom);

            _cartItemMapperMock
                .Setup(mapper => mapper.CartItemDtoToCartItem(cartItemDto, It.IsAny<CartItem>()))
                .Callback((CartItemDto dto, CartItem ci) =>
                {
                    ci.CartId = dto.CartId;
                    ci.RoomId = dto.RoomId;
                    ci.StartDate = dto.StartDate;
                    ci.EndDate = dto.EndDate;
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RoomAlreadyBookedException>(
                async () => await _cartItemService.AddCartItemAsync(cartItemDto));

            Assert.Equal("Room is not available", exception.Message);

            _cartRepositoryMock.Verify(repo => repo.GetCartByIdAsync(cartItemDto.CartId), Times.Once);
            _roomRepositoryMock.Verify(repo => repo.GetRoomByIdAsync(cartItemDto.RoomId), Times.Once);
            _cartItemRepositoryMock.Verify(repo => repo.AddCartItemAsync(It.IsAny<CartItem>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCartItemAsync_ValidCartAndItem_DeletesCartItem()
        {
            // Arrange
            int cartId = 1;
            int cartItemId = 2;

            var cartItem = _fixture.Build<CartItem>()
                .With(ci => ci.CartId, cartId)
                .With(ci => ci.CartItemId, cartItemId)
                .With(ci => ci.Price, 100)
                .Create();

            var cart = _fixture.Build<Cart>()
                .With(c => c.CartId, cartId)
                .With(c => c.CartStatus, CartStatus.Open)
                .With(c => c.CartItems, new List<CartItem> { cartItem })
                .With(c => c.TotalPrice, 200)
                .Create();

            _cartItemRepositoryMock
                .Setup(repo => repo.GetCartItemByIdAsync(cartId, cartItemId))
                .ReturnsAsync(cartItem);

            _cartRepositoryMock
                .Setup(repo => repo.GetCartByIdAsync(cartId))
                .ReturnsAsync(cart);

            _cartRepositoryMock
                .Setup(repo => repo.IsCartEmpty(cartId))
                .ReturnsAsync(true);

            // Act
            await _cartItemService.DeleteCartItemAsync(cartId, cartItemId);

            // Assert
            _cartItemRepositoryMock.Verify(repo => repo.DeleteCartItemAsync(cartItem), Times.Once);
            _cartRepositoryMock.Verify(repo => repo.DeleteCartAsync(cart), Times.Once);
            _cartRepositoryMock.Verify(repo => repo.GetCartByIdAsync(cartId), Times.Once);
        }

        [Fact]
        public async Task DeleteCartItemAsync_CartItemNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            int cartId = 1;
            int cartItemId = 2;

            _cartItemRepositoryMock
                .Setup(repo => repo.GetCartItemByIdAsync(cartId, cartItemId))
                .ReturnsAsync((CartItem)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
                async () => await _cartItemService.DeleteCartItemAsync(cartId, cartItemId));

            Assert.Equal("CartItem not found", exception.Message);

            _cartItemRepositoryMock.Verify(repo => repo.GetCartItemByIdAsync(cartId, cartItemId), Times.Once);
        }

        [Fact]
        public async Task DeleteCartItemAsync_CartIsClosed_ThrowsEntityCreationException()
        {
            // Arrange
            int cartId = 1;
            int cartItemId = 2;

            var cartItem = _fixture.Create<CartItem>();

            var closedCart = _fixture.Build<Cart>()
                .With(c => c.CartId, cartId)
                .With(c => c.CartStatus, CartStatus.Closed)
                .Create();

            _cartItemRepositoryMock
                .Setup(repo => repo.GetCartItemByIdAsync(cartId, cartItemId))
                .ReturnsAsync(cartItem);

            _cartRepositoryMock
                .Setup(repo => repo.GetCartByIdAsync(cartId))
                .ReturnsAsync(closedCart);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityCreationException>(
                async () => await _cartItemService.DeleteCartItemAsync(cartId, cartItemId));

            Assert.Equal("Cart is already closed", exception.Message);
        }

        [Fact]
        public async Task GetCartAsync_ValidCartId_ReturnsCart()
        {
            // Arrange
            int cartId = 1;

            var cart = _fixture.Build<Cart>()
                .With(c => c.CartId, cartId)
                .Create();

            _cartRepositoryMock
                .Setup(repo => repo.GetCartByIdAsync(cartId))
                .ReturnsAsync(cart);

            // Act
            var result = await _cartItemService.GetCartAsync(cartId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartId, result.CartId);
            _cartRepositoryMock.Verify(repo => repo.GetCartByIdAsync(cartId), Times.Once);
        }

        [Fact]
        public async Task GetCartAsync_CartNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            int cartId = 1;

            _cartRepositoryMock
                .Setup(repo => repo.GetCartByIdAsync(cartId))
                .ReturnsAsync((Cart)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
                async () => await _cartItemService.GetCartAsync(cartId));

            Assert.Equal("Cart not found", exception.Message);
        }

        [Fact]
        public async Task ConfirmCartAsync_ValidCartAndPayment_ClosesCartAndCreatesReservations()
        {
            // Arrange
            int cartId = 1;
            int paymentMethodId = 2;

            var cart = _fixture.Build<Cart>()
                .With(c => c.CartId, cartId)
                .With(c => c.CartStatus, CartStatus.Open)
                .With(c => c.CartItems, _fixture.CreateMany<CartItem>(3).ToList())
                .Create();

            var paymentMethod = _fixture.Create<PaymentMethod>();

            var user = _fixture.Create<User>();

            _cartRepositoryMock
                .Setup(repo => repo.GetCartByIdAsync(cartId))
                .ReturnsAsync(cart);

            _paymentMethodRepositoryMock
                .Setup(repo => repo.GetPaymentMethodByIdAsync(paymentMethodId))
                .ReturnsAsync(paymentMethod);

            _paymentMethodRepositoryMock
                .Setup(repo => repo.GetUserByPaymentMethodId(paymentMethodId))
                .ReturnsAsync(user);

            // Act
            await _cartItemService.ConfirmCartAsync(cartId, paymentMethodId);

            // Assert
            Assert.Equal(CartStatus.Closed, cart.CartStatus);
            _cartRepositoryMock.Verify(repo => repo.GetCartByIdAsync(cartId), Times.Once);
            _paymentMethodRepositoryMock.Verify(repo => repo.GetPaymentMethodByIdAsync(paymentMethodId), Times.Once);
            _reservationServiceMock.Verify(service => service.CreateReservationAsync(It.IsAny<ReservationDto>()), Times.Exactly(3));
        }

        [Fact]
        public async Task ConfirmCartAsync_PaymentMethodNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            int cartId = 1;
            int paymentMethodId = 2;

            var cart = _fixture.Build<Cart>()
                .With(c => c.CartId, cartId)
                .With(c => c.CartStatus, CartStatus.Open)
                .Create();

            _cartRepositoryMock
                .Setup(repo => repo.GetCartByIdAsync(cartId))
                .ReturnsAsync(cart);

            _paymentMethodRepositoryMock
                .Setup(repo => repo.GetPaymentMethodByIdAsync(paymentMethodId))
                .ReturnsAsync((PaymentMethod)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
                async () => await _cartItemService.ConfirmCartAsync(cartId, paymentMethodId));

            Assert.Equal("Payment method not found", exception.Message);
        }

        [Fact]
        public async Task GetUserCartsAsync_ValidUserId_ReturnsCarts()
        {
            // Arrange
            string userId = "user123";

            var carts = _fixture.CreateMany<Cart>(5).ToList();

            _cartRepositoryMock
                .Setup(repo => repo.GetUserCartsAsync(userId))
                .ReturnsAsync(carts);

            // Act
            var result = await _cartItemService.GetUserCartsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(carts.Count, result.Count);
            _cartRepositoryMock.Verify(repo => repo.GetUserCartsAsync(userId), Times.Once);
        }

    }
}