using AutoFixture;
using Moq;
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
        private readonly Mock<ICartItemRepository> _mockCartItemRepository;
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<IRoomRepository> _mockRoomRepository;
        private readonly Mock<ICartItemMapper> _mockCartItemMapper;
        private readonly Mock<IPaymentMethodRepository> _mockPaymentMethodRepository;
        private readonly Mock<IReservationService> _mockReservationService;
        private readonly CartItemService _cartItemService;
        private readonly IFixture _fixture;

        public CartItemServiceTests()
        {
            _mockCartItemRepository = new Mock<ICartItemRepository>();
            _mockCartRepository = new Mock<ICartRepository>();
            _mockRoomRepository = new Mock<IRoomRepository>();
            _mockCartItemMapper = new Mock<ICartItemMapper>();
            _mockPaymentMethodRepository = new Mock<IPaymentMethodRepository>();
            _mockReservationService = new Mock<IReservationService>();

            _cartItemService = new CartItemService(
                _mockCartItemRepository.Object,
                _mockCartRepository.Object,
                _mockRoomRepository.Object,
                _mockCartItemMapper.Object,
                _mockPaymentMethodRepository.Object,
                _mockReservationService.Object);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetCartItemByIdAsync_ShouldReturnCartItem_WhenCartItemExists()
        {
            // Arrange
            var cartItem = _fixture.Create<CartItem>();
            _mockCartItemRepository.Setup(r => r.GetCartItemByIdAsync(cartItem.CartId, cartItem.CartItemId))
                .ReturnsAsync(cartItem);

            // Act
            var result = await _cartItemService.GetCartItemByIdAsync(cartItem.CartId, cartItem.CartItemId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartItem.CartItemId, result.CartItemId);
        }

        [Fact]
        public async Task GetCartItemByIdAsync_ShouldThrowException_WhenCartItemDoesNotExist()
        {
            // Arrange
            _mockCartItemRepository.Setup(r => r.GetCartItemByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((CartItem)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _cartItemService.GetCartItemByIdAsync(1, 1));
        }

        [Fact]
        public async Task AddCartItemAsync_ShouldAddCartItem_WhenValidDataProvided()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var cartItemDto = _fixture.Create<CartItemDto>();
            var cart = _fixture.Create<Cart>();
            var room = _fixture.Create<Room>();

            _mockCartRepository.Setup(r => r.GetUserRecentCartAsync(userId)).ReturnsAsync(cart);
            _mockRoomRepository.Setup(r => r.GetRoomByIdAsync(cartItemDto.RoomId)).ReturnsAsync(room);
            _mockCartItemRepository.Setup(r => r.AddCartItemAsync(It.IsAny<CartItem>())).Returns(Task.CompletedTask);

            // Act
            var item = await _cartItemService.AddCartItemAsync(userId, cartItemDto);

            // Assert
            _mockCartItemRepository.Verify(r => r.AddCartItemAsync(It.IsAny<CartItem>()), Times.Once);
            _mockCartItemMapper.Verify(r => r.CartItemDtoToCartItem(cartItemDto, It.IsAny<CartItem>()), Times.Once);
            _mockRoomRepository.Verify(r => r.GetRoomByIdAsync(cartItemDto.RoomId), Times.Once);
        }

        [Fact]
        public async Task AddCartItemAsync_ShouldThrowException_WhenRoomNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var cartItemDto = _fixture.Create<CartItemDto>();

            _mockRoomRepository.Setup(r => r.GetRoomByIdAsync(cartItemDto.RoomId))
                .ReturnsAsync((Room)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _cartItemService.AddCartItemAsync(userId, cartItemDto));
        }

        [Fact]
        public async Task DeleteCartItemAsync_ShouldRemoveCartItem_WhenValidDataProvided()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var cart = _fixture.Create<Cart>();
            var cartItem = _fixture.Create<CartItem>();

            _mockCartRepository.Setup(r => r.GetUserRecentCartAsync(userId)).ReturnsAsync(cart);
            _mockCartItemRepository.Setup(r => r.GetCartItemByIdAsync(cart.CartId, cartItem.CartItemId)).ReturnsAsync(cartItem);
            _mockCartRepository.Setup(r => r.RemoveFromTotalPriceAsync(It.IsAny<double>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            _mockCartItemRepository.Setup(r => r.DeleteCartItemAsync(cartItem)).Returns(Task.CompletedTask);

            // Act
            await _cartItemService.DeleteCartItemAsync(userId, cartItem.CartItemId);

            // Assert
            _mockCartRepository.Verify(r => r.RemoveFromTotalPriceAsync(cartItem.Price, cart.CartId), Times.Once);
            _mockCartItemRepository.Verify(r => r.DeleteCartItemAsync(cartItem), Times.Once);
        }

        [Fact]
        public async Task ConfirmCartAsync_ShouldCloseCart_WhenValidDataProvided()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var cart = _fixture.Build<Cart>()
                .With(c => c.CartStatus, CartStatus.Open)
                .Create();
            var paymentMethod = _fixture.Build<PaymentMethod>()
                .With(p => p.UserId, userId)
                .Create();

            _mockCartRepository.Setup(r => r.GetUserRecentCartAsync(userId)).ReturnsAsync(cart);
            _mockPaymentMethodRepository.Setup(r => r.GetPaymentMethodByIdAsync(paymentMethod.PaymentMethodId))
                .ReturnsAsync(paymentMethod);
            _mockCartRepository.Setup(r => r.UpdateCartStatusAsync(cart.CartId, CartStatus.Closed))
                .Returns(Task.CompletedTask);

            // Act
            await _cartItemService.ConfirmCartAsync(userId, paymentMethod.PaymentMethodId);

            // Assert
            _mockCartRepository.Verify(r => r.UpdateCartStatusAsync(cart.CartId, CartStatus.Closed), Times.Once);
        }

        [Fact]
        public async Task ConfirmCartAsync_ShouldThrowException_WhenCartIsAlreadyClosed()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var cart = _fixture.Build<Cart>()
                .With(c => c.CartStatus, CartStatus.Closed)
                .Create();

            _mockCartRepository.Setup(r => r.GetUserRecentCartAsync(userId)).ReturnsAsync(cart);

            // Act & Assert
            await Assert.ThrowsAsync<EntityCreationException>(() =>
                _cartItemService.ConfirmCartAsync(userId, 1));
        }

        [Fact]
        public async Task GetUserCartsAsync_ShouldReturnListOfCarts_WhenCartsExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var carts = new List<Cart>
            {
                new Cart { CartId = 1, UserId = userId, CartStatus = CartStatus.Open },
                new Cart { CartId = 2, UserId = userId, CartStatus = CartStatus.Closed }
            };

            _mockCartRepository.Setup(r => r.GetUserCartsAsync(userId)).ReturnsAsync(carts);

            // Act
            var result = await _cartItemService.GetUserCartsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(carts.Count, result.Count);
            Assert.Equal(carts[0].CartId, result[0].CartId);
            Assert.Equal(carts[1].CartId, result[1].CartId);
        }

        [Fact]
        public async Task GetUserCartsAsync_ShouldReturnEmptyList_WhenNoCartsExist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            _mockCartRepository.Setup(r => r.GetUserCartsAsync(userId)).ReturnsAsync(new List<Cart>());

            // Act
            var result = await _cartItemService.GetUserCartsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCartAsync_ShouldReturnCart_WhenCartExists()
        {
            // Arrange
            var cartId = 1;
            var cart = new Cart { CartId = cartId, UserId = Guid.NewGuid().ToString(), CartStatus = CartStatus.Open };

            _mockCartRepository.Setup(r => r.GetCartByIdAsync(cartId)).ReturnsAsync(cart);

            // Act
            var result = await _cartItemService.GetCartAsync(cartId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cart.CartId, result.CartId);
            Assert.Equal(cart.UserId, result.UserId);
            Assert.Equal(cart.CartStatus, result.CartStatus);
        }

        [Fact]
        public async Task GetCartAsync_ShouldThrowEntityNotFoundException_WhenCartDoesNotExist()
        {
            // Arrange
            var cartId = 1;
            _mockCartRepository.Setup(r => r.GetCartByIdAsync(cartId)).ReturnsAsync((Cart)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _cartItemService.GetCartAsync(cartId));
        }

    }
}
