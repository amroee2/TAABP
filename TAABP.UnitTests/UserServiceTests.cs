using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.HotelMapping;
using TAABP.Application.Profile.UserMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.Services;
using TAABP.Core;

namespace TAABP.UnitTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserMapper> _userMapperMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IHotelMapper> _hotelMapperMock;
        private readonly UserService _userService;
        private readonly Fixture _fixture;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userMapperMock = new Mock<IUserMapper>();
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null
            );
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _hotelMapperMock = new Mock<IHotelMapper>();
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _userService = new UserService(
                _userRepositoryMock.Object,
                _httpContextAccessorMock.Object,
                _userMapperMock.Object,
                _userManagerMock.Object,
                _hotelMapperMock.Object
            );
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUserDto_WhenUserExists()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var user = _fixture.Create<User>();
            var userDto = _fixture.Create<UserDto>();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _userMapperMock.Setup(mapper => mapper.UserToUserDto(user)).Returns(userDto);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.Equal(userDto, result);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldThrowEntityNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var userId = _fixture.Create<string>();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.GetUserByIdAsync(userId));
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var email = _fixture.Create<string>();
            var user = _fixture.Create<User>();

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldThrowEntityNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var email = _fixture.Create<string>();

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.GetUserByEmailAsync(email));
        }

        [Fact]
        public async Task GetUsersAsync_ShouldReturnUserDtos()
        {
            // Arrange
            var users = _fixture.CreateMany<User>(3).ToList();
            var userDtos = users.Select(user => _fixture.Create<UserDto>()).ToList();

            _userRepositoryMock.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(users);
            _userMapperMock.Setup(mapper => mapper.UserToUserDto(It.IsAny<User>()))
                           .Returns((User user) => userDtos[users.IndexOf(user)]);

            // Act
            var result = await _userService.GetUsersAsync();

            // Assert
            Assert.Equal(userDtos.Count, result.Count);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldCallRepository_WhenUserExists()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var user = _fixture.Create<User>();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            await _userService.DeleteUserAsync(userId);

            // Assert
            _userRepositoryMock.Verify(repo => repo.DeleteUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldThrowEntityNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var userId = _fixture.Create<string>();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.DeleteUserAsync(userId));
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldCallRepository_WhenUserExists()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var userDto = _fixture.Create<UserDto>();
            var user = _fixture.Create<User>();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            await _userService.UpdateUserAsync(userId, userDto);

            // Assert
            _userMapperMock.Verify(mapper => mapper.UserDtoToUser(userDto, user), Times.Once);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrowEntityNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var userDto = _fixture.Create<UserDto>();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.UpdateUserAsync(userId, userDto));
        }
        [Fact]
        public async Task GetCurrentUsernameAsync_ShouldReturnUsername_WhenUserFound()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var user = _fixture.Create<User>();
            user.UserName = _fixture.Create<string>();

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            _httpContextAccessorMock.Setup(ctx => ctx.HttpContext.User).Returns(principal);
            _userManagerMock.Setup(manager => manager.FindByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetCurrentUsernameAsync();

            // Assert
            Assert.Equal(user.UserName, result);
        }


        [Fact]
        public async Task GetLastHotelsVisitedAsync_ShouldReturnHotelDtos_WhenUserExists()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var user = _fixture.Create<User>();
            var hotels = _fixture.CreateMany<Hotel>(2).ToList();
            var hotelDtos = hotels.Select(hotel => _fixture.Create<HotelDto>()).ToList();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _userRepositoryMock.Setup(repo => repo.GetLastHotelsVisitedAsync(userId)).ReturnsAsync(hotels);
            _hotelMapperMock.Setup(mapper => mapper.HotelToHotelDto(It.IsAny<Hotel>()))
                            .Returns((Hotel hotel) => hotelDtos[hotels.IndexOf(hotel)]);

            // Act
            var result = await _userService.GetLastHotelsVisitedAsync(userId);

            // Assert
            Assert.Equal(hotelDtos.Count, result.Count);
        }

        [Fact]
        public async Task GetLastHotelsVisitedAsync_ShouldThrowEntityNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var userId = _fixture.Create<string>();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.GetLastHotelsVisitedAsync(userId));
        }
    }
}
