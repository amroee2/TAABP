using Microsoft.EntityFrameworkCore;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core.PaymentEntities;

namespace TAABP.Infrastructure.Repositories.PaymentRepositories
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly TAABPDbContext _context;
        public PaymentMethodRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentMethod>> GetUserPaymentMethodsAsync(string userId)
        {
            return await _context.PaymentMethods.AsNoTracking()
                .Where(pm => pm.UserId == userId)
                .ToListAsync();
        }

        public async Task<PaymentMethod> GetPaymentMethodByIdAsync(int paymentMethodId)
        {
            return await _context.PaymentMethods.AsNoTracking()
                .FirstOrDefaultAsync(pm => pm.PaymentMethodId == paymentMethodId);
        }

        public async Task AddNewPaymentMethodAsync(PaymentMethod paymentMethod)
        {
            await _context.PaymentMethods.AddAsync(paymentMethod);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentMethodAsync(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Update(paymentMethod);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePaymentMethodAsync(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Remove(paymentMethod);
            await _context.SaveChangesAsync();
        }
    }
}
