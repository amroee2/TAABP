using Microsoft.EntityFrameworkCore;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core.PaymentEntities;

namespace TAABP.Infrastructure.Repositories.PaymentRepositories
{
    public class PayPalRepository : IPayPalRepository
    {
        private readonly TAABPDbContext _context;

        public PayPalRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public async Task<List<PayPal>> GetUserPaymentOptionAsync(string userId)
        {
            return await _context.PayPals.AsNoTracking()
                .Where(cc => cc.PaymentMethod.UserId == userId)
                .ToListAsync();
        }

        public async Task<PayPal> GetPaymentOptionByIdAsync(int creditCardId)
        {
            return await _context.PayPals.AsNoTracking()
                .FirstOrDefaultAsync(cc => cc.PayPalId == creditCardId);
        }

        public async Task AddNewPaymentOptionAsync(PayPal payPal)
        {
            await _context.PayPals.AddAsync(payPal);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentOptionAsync(PayPal payPal)
        {
            _context.PayPals.Update(payPal);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePaymentOptionAsync(PayPal payPal)
        {
            _context.PayPals.Remove(payPal);
            await _context.SaveChangesAsync();
        }
    }
}
