using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using TAABP.Core;
using TAABP.Infrastructure;
using TAABP.Infrastructure.Repositories;

namespace TAABP.IntegrationTests
{
    public class UserRepositoryTests
    {
        private readonly TAABPDbContext _context;
        private readonly UserRepository _userRepository;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly IFixture _fixture;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TAABPDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TAABPDbContext(options);

            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null, null, null, null, null, null, null, null);

            _userRepository = new UserRepository(_context, _mockUserManager.Object);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateUserAsync()
        {
            // Arrange
            var user = _fixture.Create<User>();
            var password = "SecurePassword123!";
            _mockUserManager
                .Setup(um => um.CreateAsync(user, password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userRepository.CreateUserAsync(user, password);

            // Assert
            Assert.True(result);
            _mockUserManager.Verify(um => um.CreateAsync(user, password), Times.Once);
        }

        [Fact]
        public async Task CheckEmailAsync_ShouldReturnTrueIfEmailExistsAsync()
        {
            // Arrange
            var user = _fixture.Create<User>();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.CheckEmailAsync(user.Email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckEmailAsync_ShouldReturnFalseIfEmailDoesNotExistAsync()
        {
            // Act
            var result = await _userRepository.CheckEmailAsync("nonexistent@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUserAsync()
        {
            // Arrange
            var user = _fixture.Create<User>();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByEmailAsync(user.Email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUserAsync()
        {
            // Arrange
            var user = _fixture.Create<User>();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task GetUsersAsync_ShouldReturnAllUsersAsync()
        {
            // Arrange
            var users = _fixture.CreateMany<User>(5).ToList();
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUsersAsync();

            // Assert
            Assert.Equal(users.Count, result.Count);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldRemoveUserAsync()
        {
            // Arrange
            var user = _fixture.Create<User>();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            await _userRepository.DeleteUserAsync(user);

            // Assert
            var result = await _context.Users.FindAsync(user.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldModifyUserAsync()
        {
            // Arrange
            var user = _fixture.Create<User>();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            user.FirstName = "UpdatedName";
            await _userRepository.UpdateUserAsync(user);

            // Assert
            var result = await _context.Users.FindAsync(user.Id);
            Assert.Equal("UpdatedName", result.FirstName);
        }

        [Fact]
        public async Task GetLastHotelsVisitedAsync_ShouldReturnLastVisitedHotelsAsync()
        {
            // Arrange
            var user = _fixture.Create<User>();
            user.Reservations = _fixture.CreateMany<Reservation>(5).ToList();
            foreach (var reservation in user.Reservations)
            {
                reservation.Room = _fixture.Create<Room>();
                reservation.Room.Hotel = _fixture.Create<Hotel>();
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetLastHotelsVisitedAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Count);
            Assert.All(result, hotel => Assert.NotNull(hotel));
        }
    }
}
