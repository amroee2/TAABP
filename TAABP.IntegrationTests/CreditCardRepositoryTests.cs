using AutoFixture;
using Microsoft.EntityFrameworkCore;
using TAABP.Core.PaymentEntities;
using TAABP.Infrastructure;
using TAABP.Infrastructure.Repositories.PaymentRepositories;

namespace TAABP.IntegrationTests
{
    public class CreditCardRepositoryTests
    {
        private readonly TAABPDbContext _context;
        private readonly CreditCardRepository _creditCardRepository;
        private readonly IFixture _fixture;

        public CreditCardRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TAABPDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TAABPDbContext(options);
            _creditCardRepository = new CreditCardRepository(_context);
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task AddCreditCardAsync_ShouldAddCreditCard()
        {
            // Arrange
            var creditCard = _fixture.Create<CreditCard>();

            // Act
            await _creditCardRepository.AddNewPaymentOptionAsync(creditCard);

            // Assert
            var result = await _context.CreditCards.FirstOrDefaultAsync(c => c.CreditCardId == creditCard.CreditCardId);
            Assert.NotNull(result);
            Assert.Equal(creditCard.CreditCardId, result.CreditCardId);
        }

        [Fact]
        public async Task GetCreditCardByIdAsync_ShouldReturnCreditCardById()
        {
            // Arrange
            var creditCard = _fixture.Create<CreditCard>();
            await _context.CreditCards.AddAsync(creditCard);
            await _context.SaveChangesAsync();

            // Act
            var result = await _creditCardRepository.GetPaymentOptionByIdAsync(creditCard.CreditCardId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(creditCard.CreditCardId, result.CreditCardId);
        }

        [Fact]
        public async Task GetCreditCardByPaymentMethodIdAsync_ShouldReturnCreditCardByPaymentMethodId()
        {
            // Arrange
            var creditCard = _fixture.Create<CreditCard>();
            await _context.CreditCards.AddAsync(creditCard);
            await _context.SaveChangesAsync();

            // Act
            var result = await _creditCardRepository.GetPaymentOptionByPaymentMethodId(creditCard.PaymentMethodId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(creditCard.PaymentMethodId, result.PaymentMethodId);
        }

        [Fact]
        public async Task UpdateCreditCardAsync_ShouldUpdateCreditCard()
        {
            // Arrange
            var creditCard = _fixture.Create<CreditCard>();
            await _context.CreditCards.AddAsync(creditCard);
            await _context.SaveChangesAsync();

            // Act
            creditCard.CardNumber = "1234567890123456";
            await _creditCardRepository.UpdatePaymentOptionAsync(creditCard);

            // Assert
            var result = await _context.CreditCards.FirstOrDefaultAsync(c => c.CreditCardId == creditCard.CreditCardId);
            Assert.NotNull(result);
            Assert.Equal(creditCard.CardNumber, result.CardNumber);
        }
    }
}
