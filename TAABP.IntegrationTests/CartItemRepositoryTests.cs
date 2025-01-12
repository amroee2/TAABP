using AutoFixture;
using Microsoft.EntityFrameworkCore;
using TAABP.Core.ShoppingEntities;
using TAABP.Infrastructure;
using TAABP.Infrastructure.Repositories.ShoppingRepositories;

namespace TAABP.IntegrationTests
{
    public class CartItemRepositoryTests
    {
        private readonly TAABPDbContext _context;
        private readonly CartItemRepository _cartItemRepository;
        private readonly IFixture _fixture;

        public CartItemRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TAABPDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TAABPDbContext(options);
            _cartItemRepository = new CartItemRepository(_context);
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task AddCartItemAsync_ShouldAddCartItem()
        {
            // Arrange
            var cartItem = _fixture.Create<CartItem>();

            // Act
            await _cartItemRepository.AddCartItemAsync(cartItem);

            // Assert
            var result = await _context.CartItems.FirstOrDefaultAsync(c => c.CartItemId == cartItem.CartItemId);
            Assert.NotNull(result);
            Assert.Equal(cartItem.CartItemId, result.CartItemId);
        }

        [Fact]
        public async Task GetCartItemByIdAsync_ShouldReturnCartItemById()
        {
            // Arrange
            var cartItem = _fixture.Create<CartItem>();
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cartItemRepository.GetCartItemByIdAsync(cartItem.CartId, cartItem.CartItemId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartItem.CartItemId, result.CartItemId);
        }
        [Fact]
        public async Task GetCartItemsByCartIdAsync_ShouldReturnCartItemsForCartId()
        {
            // Arrange
            var cartId = _fixture.Create<int>();

            _fixture.Customize<CartItem>(ci => ci
                .Without(c => c.Cart)
                .With(c => c.CartId, cartId)
                .With(c => c.CartItemId, () => _fixture.Create<int>()));

            var cartItems = _fixture.CreateMany<CartItem>(3).ToList();

            await _context.CartItems.AddRangeAsync(cartItems);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cartItemRepository.GetCartItemsByCartIdAsync(cartId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartItems.Count, result.Count);
            Assert.All(result, ci => Assert.Equal(cartId, ci.CartId));
            Assert.Equal(cartItems.Select(ci => ci.CartItemId), result.Select(ci => ci.CartItemId));
        }



        [Fact]
        public async Task DeleteCartItemAsync_ShouldRemoveCartItem()
        {
            // Arrange
            var cartItem = _fixture.Create<CartItem>();
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();

            // Act
            await _cartItemRepository.DeleteCartItemAsync(cartItem);

            // Assert
            var result = await _context.CartItems.FirstOrDefaultAsync(c => c.CartItemId == cartItem.CartItemId);
            Assert.Null(result);
        }
    }
}
