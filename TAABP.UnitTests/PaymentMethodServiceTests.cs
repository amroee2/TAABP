using Moq;
using TAABP.Application.Exceptions;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.Services;
using TAABP.Core.PaymentEntities;
using TAABP.Core;
using TAABP.Application.DTOs;
using Xunit;
using TAABP.Application;

namespace TAABP.UnitTests
{
    public class PaymentMethodServiceTests
    {
        private readonly Mock<IPaymentMethodRepository> _mockPaymentMethodRepository;
        private readonly Mock<IPaymentOptionServiceFactory> _mockPaymentOptionServiceFactory;
        private readonly Mock<IUserService> _mockUserService;
        private readonly PaymentMethodService _paymentMethodService;

        public PaymentMethodServiceTests()
        {
            _mockPaymentMethodRepository = new Mock<IPaymentMethodRepository>();
            _mockPaymentOptionServiceFactory = new Mock<IPaymentOptionServiceFactory>();
            _mockUserService = new Mock<IUserService>();
            _paymentMethodService = new PaymentMethodService(
                _mockPaymentMethodRepository.Object,
                _mockPaymentOptionServiceFactory.Object,
                _mockUserService.Object
            );
        }

        [Fact]
        public async Task GetAllUserPaymentOptionsAsync_ShouldReturnPaymentOptions_WhenUserExists()
        {
            // Arrange
            var userId = "test-user-id";
            var userDto = new UserDto { Id = userId, UserName = "Test User" };
            var paymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod { PaymentMethodId = 1, PaymentMethodName = PaymentMethodOption.CreditCard },
                new PaymentMethod { PaymentMethodId = 2, PaymentMethodName = PaymentMethodOption.PayPal }
            };

            _mockUserService
                .Setup(u => u.GetUserByIdAsync(userId))
                .ReturnsAsync(userDto);

            _mockPaymentMethodRepository
                .Setup(r => r.GetUserPaymentMethodsAsync(userId))
                .ReturnsAsync(paymentMethods);

            var creditCardService = new Mock<IPaymentOptionService>();
            var payPalService = new Mock<IPaymentOptionService>();

            _mockPaymentOptionServiceFactory
                .Setup(f => f.GetService(PaymentMethodOption.CreditCard))
                .Returns(creditCardService.Object);
            _mockPaymentOptionServiceFactory
                .Setup(f => f.GetService(PaymentMethodOption.PayPal))
                .Returns(payPalService.Object);

            creditCardService
                .Setup(s => s.GetPaymentOptionByPaymentMethodId(1))
                .ReturnsAsync(new CreditCard());
            payPalService
                .Setup(s => s.GetPaymentOptionByPaymentMethodId(2))
                .ReturnsAsync(new PayPal());

            // Act
            var result = await _paymentMethodService.GetAllUserPaymentOptionsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.IsType<CreditCard>(result[0]);
            Assert.IsType<PayPal>(result[1]);
        }


        [Fact]
        public async Task GetAllUserPaymentOptionsAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "invalid-user-id";

            _mockUserService
                .Setup(u => u.GetUserByIdAsync(userId))
                .ReturnsAsync((UserDto?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _paymentMethodService.GetAllUserPaymentOptionsAsync(userId));

            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task GetUserByPaymentMethodId_ShouldReturnUser_WhenPaymentMethodExists()
        {
            // Arrange
            var paymentMethodId = 1;
            var paymentMethod = new PaymentMethod { PaymentMethodId = paymentMethodId };
            var user = new User { Id = "test-user-id" };

            _mockPaymentMethodRepository
                .Setup(r => r.GetPaymentMethodByIdAsync(paymentMethodId))
                .ReturnsAsync(paymentMethod);
            _mockPaymentMethodRepository
                .Setup(r => r.GetUserByPaymentMethodId(paymentMethodId))
                .ReturnsAsync(user);

            // Act
            var result = await _paymentMethodService.GetUserByPaymentMethodId(paymentMethodId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task GetUserByPaymentMethodId_ShouldThrowException_WhenPaymentMethodDoesNotExist()
        {
            // Arrange
            var paymentMethodId = 999;

            _mockPaymentMethodRepository
                .Setup(r => r.GetPaymentMethodByIdAsync(paymentMethodId))
                .ReturnsAsync((PaymentMethod?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _paymentMethodService.GetUserByPaymentMethodId(paymentMethodId));

            Assert.Equal("Payment method not found", exception.Message);
        }
    }
}
