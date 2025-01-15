using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.PayPalMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.Services;
using TAABP.Core;
using TAABP.Core.PaymentEntities;

namespace TAABP.UnitTests
{
    public class PayPalServiceTests
    {
        private readonly PayPalService _payPalService;
        private readonly Mock<IPayPalRepository> _payPalRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPaymentMethodRepository> _paymentMethodRepositoryMock;
        private readonly Mock<IPayPalMapper> _payPalMapperMock;
        private readonly IFixture _fixture;

        public PayPalServiceTests()
        {
            _payPalRepositoryMock = new Mock<IPayPalRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _paymentMethodRepositoryMock = new Mock<IPaymentMethodRepository>();
            _payPalMapperMock = new Mock<IPayPalMapper>();

            _payPalService = new PayPalService(_payPalRepositoryMock.Object, _userRepositoryMock.Object,
                _paymentMethodRepositoryMock.Object, _payPalMapperMock.Object);

            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        [Fact]
        public async Task GetPaymentOptionByPaymentMethodId_ValidId_ReturnsPaymentOption()
        {
            // Arrange
            var paymentMethodId = _fixture.Create<int>();
            var expectedPayPal = _fixture.Create<PayPal>();
            _payPalRepositoryMock
                .Setup(repo => repo.GetPaymentOptionByPaymentMethodId(paymentMethodId))
                .ReturnsAsync(expectedPayPal);

            // Act
            var result = await _payPalService.GetPaymentOptionByPaymentMethodId(paymentMethodId);

            // Assert
            Assert.Equal(expectedPayPal, result);
            _payPalRepositoryMock.Verify(repo => repo.GetPaymentOptionByPaymentMethodId(paymentMethodId), Times.Once);
        }

        [Fact]
        public async Task GetPaymentOptionByIdAsync_InvalidId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var invalidId = _fixture.Create<int>();
            _payPalRepositoryMock
                .Setup(repo => repo.GetPaymentOptionByIdAsync(invalidId))
                .ReturnsAsync((PayPal)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _payPalService.GetPaymentOptionByIdAsync(invalidId));
        }

        [Fact]
        public async Task AddNewPaymentOptionAsync_ValidInput_ReturnsPayPalId()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var payPalDto = _fixture.Create<PayPalDto>();
            var user = _fixture.Create<User>();
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _payPalRepositoryMock.Setup(repo => repo.CheckIfEmailAlreadyExists(payPalDto.PayPalEmail)).ReturnsAsync(false);
            _paymentMethodRepositoryMock.Setup(repo => repo.AddNewPaymentMethodAsync(It.IsAny<PaymentMethod>()));
            _payPalMapperMock.Setup(mapper => mapper.PayPalDtoToPayPal(payPalDto, It.IsAny<PayPal>()));

            var expectedPayPalId = _fixture.Create<int>();
            _payPalRepositoryMock
                .Setup(repo => repo.AddNewPaymentOptionAsync(It.IsAny<PayPal>()))
                .Callback<PayPal>(paypal => paypal.PayPalId = expectedPayPalId)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _payPalService.AddNewPaymentOptionAsync(userId, payPalDto);

            // Assert
            Assert.Equal(expectedPayPalId, result);
        }

        [Fact]
        public async Task AddNewPaymentOptionAsync_EmailAlreadyExists_ThrowsEmailAlreadyExistsException()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var payPalDto = _fixture.Create<PayPalDto>();
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(new User());
            _payPalRepositoryMock.Setup(repo => repo.CheckIfEmailAlreadyExists(payPalDto.PayPalEmail)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<EmailAlreadyExistsException>(
                () => _payPalService.AddNewPaymentOptionAsync(userId, payPalDto));
        }

        [Fact]
        public async Task UpdatePaymentOptionAsync_ValidInputs_UpdatesPaymentOption()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var payPalId = _fixture.Create<int>();
            var paymentMethodId = _fixture.Create<int>();
            var user = _fixture.Create<User>();
            var existingPayPal = _fixture.Build<PayPal>()
                .With(p => p.PaymentMethodId, paymentMethodId)
                .Create();
            var paymentMethod = _fixture.Build<PaymentMethod>()
                .With(pm => pm.UserId, userId)
                .Create();
            var payPalDto = _fixture.Build<PayPalDto>()
                .With(dto => dto.PaymentMethodId, paymentMethodId)
                .With(dto => dto.PayPalEmail, "updated_email@example.com")
                .Create();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _payPalRepositoryMock.Setup(repo => repo.GetPaymentOptionByIdAsync(payPalId)).ReturnsAsync(existingPayPal);
            _paymentMethodRepositoryMock.Setup(repo => repo.GetPaymentMethodByIdAsync(existingPayPal.PaymentMethodId)).ReturnsAsync(paymentMethod);
            _payPalRepositoryMock.Setup(repo => repo.CheckIfEmailAlreadyExists(payPalDto.PayPalEmail)).ReturnsAsync(false);
            _payPalRepositoryMock.Setup(repo => repo.UpdatePaymentOptionAsync(existingPayPal)).Returns(Task.CompletedTask);

            _payPalMapperMock.Setup(mapper => mapper.PayPalDtoToPayPal(payPalDto, existingPayPal))
                .Callback<PayPalDto, PayPal>((dto, paypal) => paypal.PayPalEmail = dto.PayPalEmail);

            // Act
            await _payPalService.UpdatePaymentOptionAsync(payPalId, userId, payPalDto);

            // Assert
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
            _payPalRepositoryMock.Verify(repo => repo.GetPaymentOptionByIdAsync(payPalId), Times.Once);
            _paymentMethodRepositoryMock.Verify(repo => repo.GetPaymentMethodByIdAsync(existingPayPal.PaymentMethodId), Times.Once);
            _payPalRepositoryMock.Verify(repo => repo.CheckIfEmailAlreadyExists(payPalDto.PayPalEmail), Times.Once);
            _payPalMapperMock.Verify(mapper => mapper.PayPalDtoToPayPal(payPalDto, existingPayPal), Times.Once);
            _payPalRepositoryMock.Verify(repo => repo.UpdatePaymentOptionAsync(existingPayPal), Times.Once);
        }


        [Fact]
        public async Task UpdatePaymentOptionAsync_InvalidPayPalId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var payPalId = _fixture.Create<int>();
            var userId = _fixture.Create<string>();
            var payPalDto = _fixture.Create<PayPalDto>();
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(new User());
            _payPalRepositoryMock.Setup(repo => repo.GetPaymentOptionByIdAsync(payPalId)).ReturnsAsync((PayPal)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _payPalService.UpdatePaymentOptionAsync(payPalId, userId, payPalDto));
        }

        [Fact]
        public async Task DeletePaymentOptionAsync_ValidUserIdAndPaymentOptionId_DeletesPaymentMethod()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var paymentOptionId = _fixture.Create<int>();
            var user = _fixture.Create<User>();
            var payPal = _fixture.Build<PayPal>()
                .With(p => p.PaymentMethodId, _fixture.Create<int>())
                .Create();
            var paymentMethod = _fixture.Build<PaymentMethod>()
                .With(pm => pm.UserId, userId)
                .Create();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _payPalRepositoryMock.Setup(repo => repo.GetPaymentOptionByIdAsync(paymentOptionId)).ReturnsAsync(payPal);
            _paymentMethodRepositoryMock.Setup(repo => repo.GetPaymentMethodByIdAsync(payPal.PaymentMethodId)).ReturnsAsync(paymentMethod);
            _paymentMethodRepositoryMock.Setup(repo => repo.DeletePaymentMethodAsync(paymentMethod)).Returns(Task.CompletedTask);

            // Act
            await _payPalService.DeletePaymentOptionAsync(userId, paymentOptionId);

            // Assert
            _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
            _payPalRepositoryMock.Verify(repo => repo.GetPaymentOptionByIdAsync(paymentOptionId), Times.Once);
            _paymentMethodRepositoryMock.Verify(repo => repo.GetPaymentMethodByIdAsync(payPal.PaymentMethodId), Times.Once);
            _paymentMethodRepositoryMock.Verify(repo => repo.DeletePaymentMethodAsync(paymentMethod), Times.Once);
        }


        [Fact]
        public async Task DeletePaymentOptionAsync_InvalidUserId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var paymentOptionId = _fixture.Create<int>();
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _payPalService.DeletePaymentOptionAsync(userId, paymentOptionId));
        }

        [Fact]
        public async Task DeletePaymentOptionAsync_InvalidPaymentOptionId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var paymentOptionId = _fixture.Create<int>();
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(new User());
            _payPalRepositoryMock.Setup(repo => repo.GetPaymentOptionByIdAsync(paymentOptionId)).ReturnsAsync((PayPal)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _payPalService.DeletePaymentOptionAsync(userId, paymentOptionId));
        }
    }
}
