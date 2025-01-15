using AutoFixture;
using Microsoft.EntityFrameworkCore;
using TAABP.Core.PaymentEntities;
using TAABP.Infrastructure;
using TAABP.Infrastructure.Repositories.PaymentRepositories;

namespace TAABP.IntegrationTests
{
    public class PaymentMethodRepositoryTests
    {
        private readonly TAABPDbContext _context;
        private readonly PaymentMethodRepository _paymentMethodRepository;
        private readonly IFixture _fixture;

        public PaymentMethodRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TAABPDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TAABPDbContext(options);
            _paymentMethodRepository = new PaymentMethodRepository(_context);
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task AddNewPaymentMethodAsync_ShouldAddPaymentMethod()
        {
            // Arrange
            var paymentMethod = _fixture.Create<PaymentMethod>();

            // Act
            await _paymentMethodRepository.AddNewPaymentMethodAsync(paymentMethod);

            // Assert
            var result = await _context.PaymentMethods.FirstOrDefaultAsync(pm => pm.PaymentMethodId == paymentMethod.PaymentMethodId);
            Assert.NotNull(result);
            Assert.Equal(paymentMethod.PaymentMethodId, result.PaymentMethodId);
        }

        [Fact]
        public async Task GetPaymentMethodByIdAsync_ShouldReturnPaymentMethodById()
        {
            // Arrange
            var paymentMethod = _fixture.Create<PaymentMethod>();
            await _context.PaymentMethods.AddAsync(paymentMethod);
            await _context.SaveChangesAsync();

            // Act
            var result = await _paymentMethodRepository.GetPaymentMethodByIdAsync(paymentMethod.PaymentMethodId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paymentMethod.PaymentMethodId, result.PaymentMethodId);
        }

        [Fact]
        public async Task GetUserPaymentMethodsAsync_ShouldReturnPaymentMethodsForUserId()
        {
            // Arrange
            var paymentMethod = _fixture.Create<PaymentMethod>();
            await _context.PaymentMethods.AddAsync(paymentMethod);
            await _context.SaveChangesAsync();

            // Act
            var result = await _paymentMethodRepository.GetUserPaymentMethodsAsync(paymentMethod.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(paymentMethod.UserId, result[0].UserId);
        }

        [Fact]
        public async Task DeletePaymentMethodAsync_ShouldDeletePaymentMethod()
        {
            // Arrange
            var paymentMethod = _fixture.Create<PaymentMethod>();
            await _context.PaymentMethods.AddAsync(paymentMethod);
            await _context.SaveChangesAsync();

            // Act
            await _paymentMethodRepository.DeletePaymentMethodAsync(paymentMethod);

            // Assert
            var result = await _context.PaymentMethods.FirstOrDefaultAsync(pm => pm.PaymentMethodId == paymentMethod.PaymentMethodId);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByPaymentMethodId_ShouldReturnUserByPaymentMethodId()
        {
            // Arrange
            var paymentMethod = _fixture.Create<PaymentMethod>();
            await _context.PaymentMethods.AddAsync(paymentMethod);
            await _context.SaveChangesAsync();

            // Act
            var result = await _paymentMethodRepository.GetUserByPaymentMethodId(paymentMethod.PaymentMethodId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paymentMethod.UserId, result.Id);
        }
    }
}
