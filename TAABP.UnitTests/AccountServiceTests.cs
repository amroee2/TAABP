using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using TAABP.Application.DTOs;
using TAABP.Application.DTOs.AccountDto;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.UserMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.Services;
using TAABP.Application.TokenGenerators;
using TAABP.Core;

namespace TAABP.UnitTests
{
    public class AccountServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserMapper> _userMapperMock;
        private readonly Mock<ITokenGenerator> _tokenGeneratorMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _userRepositoryMock = new Mock<IUserRepository>();
            _userMapperMock = new Mock<IUserMapper>();
            _tokenGeneratorMock = new Mock<ITokenGenerator>();
            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                null, null, null, null);

            _accountService = new AccountService(
                _userManagerMock.Object,
                _userRepositoryMock.Object,
                _userMapperMock.Object,
                _tokenGeneratorMock.Object,
                _signInManagerMock.Object);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateUser_WhenEmailDoesNotExist()
        {
            //Arrange
            var registerDto = _fixture.Create<RegisterDto>();
            var user = _fixture.Create<User>();

            _userRepositoryMock.Setup(repo => repo.CheckEmailAsync(registerDto.Email)).ReturnsAsync(false);
            _userMapperMock.Setup(mapper => mapper.RegisterDtoToUser(registerDto)).Returns(user);
            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(user, registerDto.Password)).ReturnsAsync(true);

            //Act
            await _accountService.CreateUserAsync(registerDto);

            //Assert
            _userRepositoryMock.Verify(repo => repo.CreateUserAsync(user, registerDto.Password), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldThrowEmailAlreadyExistsException_WhenEmailExists()
        {
            //Arrange
            var registerDto = _fixture.Create<RegisterDto>();

            //Act
            _userRepositoryMock.Setup(repo => repo.CheckEmailAsync(registerDto.Email)).ReturnsAsync(true);

            //Assert
            await Assert.ThrowsAsync<EmailAlreadyExistsException>(() => _accountService.CreateUserAsync(registerDto));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            //Arrange
            var loginDto = _fixture.Create<LoginDto>();
            var user = _fixture.Create<User>();
            var token = _fixture.Create<string>();

            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(signIn => signIn.CheckPasswordSignInAsync(user, loginDto.Password, false))
                .ReturnsAsync(SignInResult.Success);
            _tokenGeneratorMock.Setup(gen => gen.GenerateToken(user.Id)).Returns(token);

            //Act
            var result = await _accountService.LoginAsync(loginDto);

            //Assert
            Assert.Equal(token, result);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowInvalidLoginException_WhenCredentialsAreInvalid()
        {
            //Arrange
            var loginDto = _fixture.Create<LoginDto>();

            //Act
            _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync((User)null);

            //Assert
            await Assert.ThrowsAsync<InvalidLoginException>(() => _accountService.LoginAsync(loginDto));
        }

        [Fact]
        public async Task ChangeEmailAsync_ShouldChangeEmail_WhenDataIsValid()
        {
            //Arrange
            var userId = _fixture.Create<string>();
            var changeEmailDto = _fixture.Create<ChangeEmailDto>();
            var user = _fixture.Create<User>();
            user.Email = _fixture.Create<string>();

            _userManagerMock.Setup(manager => manager.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(manager => manager.CheckPasswordAsync(user, changeEmailDto.Password)).ReturnsAsync(true);
            _userRepositoryMock.Setup(repo => repo.CheckEmailAsync(changeEmailDto.NewEmail)).ReturnsAsync(false);
            _userManagerMock.Setup(manager => manager.GenerateChangeEmailTokenAsync(user, changeEmailDto.NewEmail))
                .ReturnsAsync("token");
            _userManagerMock.Setup(manager => manager.ChangeEmailAsync(user, changeEmailDto.NewEmail, "token"))
                .ReturnsAsync(IdentityResult.Success);

            //Act
            await _accountService.ChangeEmailAsync(userId, changeEmailDto);

            //Assert
            _userManagerMock.Verify(manager => manager.ChangeEmailAsync(user, changeEmailDto.NewEmail, "token"), Times.Once);
        }

        [Fact]
        public async Task ResetUserPasswordAsync_ShouldResetPassword_WhenUserExists()
        {
            //Arrange
            var email = _fixture.Create<string>();
            var password = _fixture.Create<string>();
            var user = _fixture.Create<User>();

            _userManagerMock.Setup(manager => manager.FindByEmailAsync(email)).ReturnsAsync(user);
            _userManagerMock.Setup(manager => manager.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("token");
            _userManagerMock.Setup(manager => manager.ResetPasswordAsync(user, "token", password))
                .ReturnsAsync(IdentityResult.Success);

            //Act
            await _accountService.ResetUserPasswordAsync(email, password);

            //Assert
            _userManagerMock.Verify(manager => manager.ResetPasswordAsync(user, "token", password), Times.Once);
        }

        [Fact]
        public async Task ResetUserPasswordAsync_ShouldThrowEntityNotFoundException_WhenUserDoesNotExist()
        {
            //Arrange
            var email = _fixture.Create<string>();
            var password = _fixture.Create<string>();

            //Act
            _userManagerMock.Setup(manager => manager.FindByEmailAsync(email)).ReturnsAsync((User)null);

            //Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _accountService.ResetUserPasswordAsync(email, password));
        }
    }
}