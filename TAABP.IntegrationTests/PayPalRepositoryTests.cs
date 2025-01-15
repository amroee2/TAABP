using AutoFixture;
using Microsoft.EntityFrameworkCore;
using TAABP.Core.PaymentEntities;
using TAABP.Infrastructure;
using TAABP.Infrastructure.Repositories.PaymentRepositories;

namespace TAABP.IntegrationTests
{
    public class PayPalRepositoryTests
    {
        private readonly TAABPDbContext _context;
        private readonly PayPalRepository _payPalRepository;
        private readonly IFixture _fixture;

        public PayPalRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TAABPDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TAABPDbContext(options);
            _payPalRepository = new PayPalRepository(_context);
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task AddPayPalAsync_ShouldAddPayPal()
        {
            // Arrange
            var payPal = _fixture.Create<PayPal>();

            // Act
            await _payPalRepository.AddNewPaymentOptionAsync(payPal);

            // Assert
            var result = await _context.PayPals.FirstOrDefaultAsync(p => p.PayPalId == payPal.PayPalId);
            Assert.NotNull(result);
            Assert.Equal(payPal.PayPalId, result.PayPalId);
        }

        [Fact]
        public async Task GetPayPalByIdAsync_ShouldReturnPayPalById()
        {
            // Arrange
            var payPal = _fixture.Create<PayPal>();
            await _context.PayPals.AddAsync(payPal);
            await _context.SaveChangesAsync();

            // Act
            var result = await _payPalRepository.GetPaymentOptionByIdAsync(payPal.PayPalId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(payPal.PayPalId, result.PayPalId);
        }

        [Fact]
        public async Task GetPayPalByPaymentMethodIdAsync_ShouldReturnPayPalByPaymentMethodId()
        {
            // Arrange
            var payPal = _fixture.Create<PayPal>();
            await _context.PayPals.AddAsync(payPal);
            await _context.SaveChangesAsync();

            // Act
            var result = await _payPalRepository.GetPaymentOptionByPaymentMethodId(payPal.PaymentMethodId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(payPal.PaymentMethodId, result.PaymentMethodId);
        }

        [Fact]
        public async Task CheckIfEmailAlreadyExists_ShouldReturnTrueIfEmailExists()
        {
            // Arrange
            var payPal = _fixture.Create<PayPal>();
            await _context.PayPals.AddAsync(payPal);
            await _context.SaveChangesAsync();

            // Act
            var result = await _payPalRepository.CheckIfEmailAlreadyExists(payPal.PayPalEmail);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckIfEmailAlreadyExists_ShouldReturnFalseIfEmailDoesNotExist()
        {
            // Arrange // Act
            var result = await _payPalRepository.CheckIfEmailAlreadyExists(_fixture.Create<string>());

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdatePayPalAsync_ShouldUpdatePayPal()
        {
            // Arrange
            var payPal = _fixture.Create<PayPal>();
            await _context.PayPals.AddAsync(payPal);
            await _context.SaveChangesAsync();

            // Act
            payPal.PayPalEmail = _fixture.Create<string>();
            await _payPalRepository.UpdatePaymentOptionAsync(payPal);

            // Assert
            var result = await _context.PayPals.FirstOrDefaultAsync(p => p.PayPalId == payPal.PayPalId);
            Assert.NotNull(result);
            Assert.Equal(payPal.PayPalEmail, result.PayPalEmail);
        }
    }
}
