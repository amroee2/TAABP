using AutoFixture;
using Microsoft.EntityFrameworkCore;
using TAABP.Core;
using TAABP.Core.ShoppingEntities;
using TAABP.Infrastructure;
using TAABP.Infrastructure.Repositories.ShoppingRepositories;

namespace TAABP.IntegrationTests
{
    public class CartRepositoryTests
    {
        private readonly TAABPDbContext _context;
        private readonly CartRepository _cartRepository;
        private readonly IFixture _fixture;
        public CartRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TAABPDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TAABPDbContext(options);

            _cartRepository = new CartRepository(_context);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task AddCartAsync_ShouldAddCart()
        {
            // Arrange
            var cart = _fixture.Create<Cart>();

            // Act
            await _cartRepository.AddCartAsync(cart);

            // Assert
            var result = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cart.CartId);
            Assert.NotNull(result);
            Assert.Equal(cart.CartId, result.CartId);
        }

        [Fact]
        public async Task GetCartByIdAsync_ShouldReturnCartById()
        {
            // Arrange
            var cart = _fixture.Create<Cart>();
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cartRepository.GetCartByIdAsync(cart.CartId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cart.CartId, result.CartId);
        }

        [Fact]
        public async Task GetUserCartsAsync_ShouldReturnUserCarts()
        {
            // Arrange
            var cart = _fixture.Create<Cart>();
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cartRepository.GetUserCartsAsync(cart.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task IsCartEmpty_ShouldReturnTrueIfCartIsEmpty()
        {
            // Arrange
            _fixture.Customize<Cart>(c => c.Without(ca => ca.CartItems));
            var cart = _fixture.Create<Cart>();
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cartRepository.IsCartEmpty(cart.CartId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsRoomAlreadyInCart_ShouldReturnTrueIfRoomIsAlreadyInCart()
        {
            // Arrange
            var cart = _fixture.Create<Cart>();
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cartRepository.IsRoomAlreadyInCart(cart.CartId, cart.CartItems[0].RoomId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteCartAsync_ShouldDeleteCart()
        {
            // Arrange
            var cart = _fixture.Create<Cart>();
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            // Act
            await _cartRepository.DeleteCartAsync(cart);

            // Assert
            var result = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cart.CartId);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCartByIdAsync_ShouldReturnCartWithCartItems()
        {
            // Arrange
            var cart = _fixture.Create<Cart>();
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cartRepository.GetCartByIdAsync(cart.CartId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cart.CartItems.Count, result.CartItems.Count);
        }

        [Fact]
        public async Task GetUserRecentCartAsync_ShouldReturnMostRecentCart()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = _fixture.Build<User>()
                .With(u => u.Id, userId)
                .Create();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            var carts = _fixture.Build<Cart>()
                .With(c => c.UserId, userId)
                .With(c => c.CartStatus, CartStatus.Open)
                .Without(c => c.User)
                .Without(c => c.CartItems)
                .CreateMany(3).ToList();

            await _context.Carts.AddRangeAsync(carts);
            await _context.SaveChangesAsync();

            var result = await _cartRepository.GetUserRecentCartAsync(userId);
            var maxCartId = await _context.Carts
                .Where(c => c.UserId == userId)
                .MaxAsync(c => c.CartId);

            // Assert
            Assert.Equal(maxCartId, result.CartId);
        }

        [Fact]
        public async Task AddToTotalPriceAsync_ShouldIncreaseTotalPrice()
        {
            // Arrange
            var cart = _fixture.Create<Cart>();
            cart.TotalPrice = 50;
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            // Act
            await _cartRepository.AddToTotalPriceAsync(25, cart.CartId);

            // Assert
            var updatedCart = await _context.Carts.FindAsync(cart.CartId);
            Assert.Equal(75, updatedCart.TotalPrice);
        }

        [Fact]
        public async Task RemoveFromTotalPriceAsync_ShouldDecreaseTotalPrice()
        {
            // Arrange
            var cart = _fixture.Create<Cart>();
            cart.TotalPrice = 50;
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            // Act
            await _cartRepository.RemoveFromTotalPriceAsync(20, cart.CartId);

            // Assert
            var updatedCart = await _context.Carts.FindAsync(cart.CartId);
            Assert.Equal(30, updatedCart.TotalPrice);
        }

        [Fact]
        public async Task UpdateCartStatusAsync_ShouldUpdateCartStatus()
        {
            // Arrange
            var cart = _fixture.Create<Cart>();
            cart.CartStatus = CartStatus.Open;
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            // Act
            await _cartRepository.UpdateCartStatusAsync(cart.CartId, CartStatus.Closed);

            // Assert
            var updatedCart = await _context.Carts.FindAsync(cart.CartId);
            Assert.Equal(CartStatus.Closed, updatedCart.CartStatus);
        }
    }
}
