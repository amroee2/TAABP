using AutoFixture;
using Moq;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.CreditCardMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.Services;
using TAABP.Core;
using TAABP.Core.PaymentEntities;

namespace TAABP.UnitTests
{
    public class CreditCardServiceTests
    {
        private Mock<ICreditCardRepository> _mockCreditCardRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IPaymentMethodRepository> _mockPaymentMethodRepository;
        private Mock<ICreditCardMapper> _mockCreditCardMapper;
        private CreditCardService _creditCardService;
        private readonly IFixture _fixture;

        public CreditCardServiceTests()
        {
            _mockCreditCardRepository = new Mock<ICreditCardRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPaymentMethodRepository = new Mock<IPaymentMethodRepository>();
            _mockCreditCardMapper = new Mock<ICreditCardMapper>();
            _creditCardService = new CreditCardService(_mockCreditCardRepository.Object, _mockUserRepository.Object,
            _mockPaymentMethodRepository.Object, _mockCreditCardMapper.Object);
            _fixture = new Fixture();
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
            var creditCard = _fixture.Create<CreditCard>();

            _mockCreditCardRepository
                .Setup(repo => repo.GetPaymentOptionByPaymentMethodId(paymentMethodId))
                .ReturnsAsync(creditCard);

            // Act
            var result = await _creditCardService.GetPaymentOptionByPaymentMethodId(paymentMethodId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CreditCard>(result);
            Assert.Equal(creditCard, result);
            _mockCreditCardRepository.Verify(repo => repo.GetPaymentOptionByPaymentMethodId(paymentMethodId), Times.Once);
        }

        [Fact]
        public async Task GetPaymentOptionByPaymentMethodId_InvalidId_ThrowsException()
        {
            // Arrange
            var paymentMethodId = _fixture.Create<int>();

            _mockCreditCardRepository
                .Setup(repo => repo.GetPaymentOptionByPaymentMethodId(paymentMethodId))
                .ReturnsAsync((CreditCard)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _creditCardService.GetPaymentOptionByPaymentMethodId(paymentMethodId));
        }

        [Fact]
        public async Task GetPaymentOptionByIdAsync_ValidId_ReturnsPaymentOption()
        {
            // Arrange
            var paymentOptionId = _fixture.Create<int>();
            var creditCard = _fixture.Create<CreditCard>();

            _mockCreditCardRepository
                .Setup(repo => repo.GetPaymentOptionByIdAsync(paymentOptionId))
                .ReturnsAsync(creditCard);

            // Act
            var result = await _creditCardService.GetPaymentOptionByIdAsync(paymentOptionId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CreditCard>(result);
            Assert.Equal(creditCard, result);
            _mockCreditCardRepository.Verify(repo => repo.GetPaymentOptionByIdAsync(paymentOptionId), Times.Once);
        }

        [Fact]
        public async Task GetPaymentOptionByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var paymentOptionId = _fixture.Create<int>();

            _mockCreditCardRepository
                .Setup(repo => repo.GetPaymentOptionByIdAsync(paymentOptionId))
                .ReturnsAsync((CreditCard)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _creditCardService.GetPaymentOptionByIdAsync(paymentOptionId));
        }

        [Fact]
        public async Task AddNewPaymentOptionAsync_ValidData_ReturnsCreditCardId()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var paymentOptionDto = _fixture.Create<CreditCardDto>();
            var user = _fixture.Create<User>();
            var paymentMethod = _fixture.Create<PaymentMethod>();
            var creditCard = _fixture.Create<CreditCard>();

            _mockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _mockPaymentMethodRepository
                .Setup(repo => repo.AddNewPaymentMethodAsync(It.IsAny<PaymentMethod>()))
                .Callback<PaymentMethod>(method => paymentMethod = method);

            _mockCreditCardMapper
                .Setup(mapper => mapper.CreditCardDtoToCreditCard(paymentOptionDto, It.IsAny<CreditCard>()))
                .Callback((CreditCardDto dto, CreditCard cc) => { cc.PaymentMethodId = paymentMethod.PaymentMethodId; });

            _mockCreditCardRepository
                .Setup(repo => repo.AddNewPaymentOptionAsync(It.IsAny<CreditCard>()))
                .Callback<CreditCard>(cc => creditCard = cc);

            // Act
            var result = await _creditCardService.AddNewPaymentOptionAsync(userId, paymentOptionDto);

            // Assert
            Assert.Equal(creditCard.CreditCardId, result);
            _mockUserRepository.Verify(repo => repo.GetUserByIdAsync(userId), Times.Once);
            _mockCreditCardRepository.Verify(repo => repo.AddNewPaymentOptionAsync(It.IsAny<CreditCard>()), Times.Once);
        }

        [Fact]
        public async Task AddNewPaymentOptionAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var paymentOptionDto = _fixture.Create<CreditCardDto>();

            _mockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _creditCardService.AddNewPaymentOptionAsync(userId, paymentOptionDto));
        }

        [Fact]
        public async Task UpdatePaymentOptionAsync_ValidData_UpdatesCreditCard()
        {
            // Arrange
            var creditCardId = _fixture.Create<int>();
            var userId = _fixture.Create<string>();
            var paymentOptionDto = _fixture.Create<CreditCardDto>();
            var user = _fixture.Create<User>();
            var creditCard = _fixture.Create<CreditCard>();
            var paymentMethod = _fixture.Build<PaymentMethod>()
                .With(pm => pm.UserId, userId)
                .Create();

            _mockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _mockCreditCardRepository
                .Setup(repo => repo.GetPaymentOptionByIdAsync(creditCardId))
                .ReturnsAsync(creditCard);

            _mockPaymentMethodRepository
                .Setup(repo => repo.GetPaymentMethodByIdAsync(creditCard.PaymentMethodId))
                .ReturnsAsync(paymentMethod);

            // Act
            await _creditCardService.UpdatePaymentOptionAsync(creditCardId, userId, paymentOptionDto);

            // Assert
            _mockCreditCardRepository.Verify(repo => repo.UpdatePaymentOptionAsync(creditCard), Times.Once);
        }

        [Fact]
        public async Task UpdatePaymentOptionAsync_UserMismatch_ThrowsException()
        {
            // Arrange
            var creditCardId = _fixture.Create<int>();
            var userId = _fixture.Create<string>();
            var paymentOptionDto = _fixture.Create<CreditCardDto>();
            var creditCard = _fixture.Create<CreditCard>();
            var paymentMethod = _fixture.Build<PaymentMethod>()
                .With(pm => pm.UserId, _fixture.Create<string>())
                .Create();

            _mockCreditCardRepository
                .Setup(repo => repo.GetPaymentOptionByIdAsync(creditCardId))
                .ReturnsAsync(creditCard);

            _mockPaymentMethodRepository
                .Setup(repo => repo.GetPaymentMethodByIdAsync(creditCard.PaymentMethodId))
                .ReturnsAsync(paymentMethod);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _creditCardService.UpdatePaymentOptionAsync(creditCardId, userId, paymentOptionDto));
        }

        [Fact]
        public async Task DeletePaymentOptionAsync_ValidData_DeletesPaymentOption()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var paymentOptionId = _fixture.Create<int>();
            var user = _fixture.Create<User>();
            var creditCard = _fixture.Create<CreditCard>();
            var paymentMethod = _fixture.Build<PaymentMethod>()
                .With(pm => pm.UserId, userId)
                .Create();

            _mockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _mockCreditCardRepository
                .Setup(repo => repo.GetPaymentOptionByIdAsync(paymentOptionId))
                .ReturnsAsync(creditCard);

            _mockPaymentMethodRepository
                .Setup(repo => repo.GetPaymentMethodByIdAsync(creditCard.PaymentMethodId))
                .ReturnsAsync(paymentMethod);

            _mockPaymentMethodRepository
                .Setup(repo => repo.DeletePaymentMethodAsync(paymentMethod))
                .Returns(Task.CompletedTask);

            // Act
            await _creditCardService.DeletePaymentOptionAsync(userId, paymentOptionId);

            // Assert
            _mockPaymentMethodRepository.Verify(repo => repo.DeletePaymentMethodAsync(paymentMethod), Times.Once);
        }

        [Fact]
        public async Task DeletePaymentOptionAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var paymentOptionId = _fixture.Create<int>();

            _mockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _creditCardService.DeletePaymentOptionAsync(userId, paymentOptionId));
        }

        [Fact]
        public async Task DeletePaymentOptionAsync_CreditCardNotFound_ThrowsException()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var paymentOptionId = _fixture.Create<int>();
            var user = _fixture.Create<User>();

            _mockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _mockCreditCardRepository
                .Setup(repo => repo.GetPaymentOptionByIdAsync(paymentOptionId))
                .ReturnsAsync((CreditCard)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _creditCardService.DeletePaymentOptionAsync(userId, paymentOptionId));
        }

        [Fact]
        public async Task DeletePaymentOptionAsync_PaymentMethodNotFound_ThrowsException()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var paymentOptionId = _fixture.Create<int>();
            var user = _fixture.Create<User>();
            var creditCard = _fixture.Create<CreditCard>();

            _mockUserRepository
                .Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _mockCreditCardRepository
                .Setup(repo => repo.GetPaymentOptionByIdAsync(paymentOptionId))
                .ReturnsAsync(creditCard);

            _mockPaymentMethodRepository
                .Setup(repo => repo.GetPaymentMethodByIdAsync(creditCard.PaymentMethodId))
                .ReturnsAsync((PaymentMethod)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _creditCardService.DeletePaymentOptionAsync(userId, paymentOptionId));
        }
    }
}
