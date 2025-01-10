using Microsoft.EntityFrameworkCore;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core.PaymentEntities;

namespace TAABP.Infrastructure.Repositories.PaymentRepositories
{
    public class CreditCardRepository : ICreditCardRepository
    {
        private readonly TAABPDbContext _context;
        public CreditCardRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public async Task<List<CreditCard>> GetUserPaymentOptionAsync(string userId)
        {
            return await _context.CreditCards.AsNoTracking()
                .Where(cc => cc.PaymentMethod.UserId == userId)
                .ToListAsync();
        }

        public async Task<CreditCard> GetPaymentOptionByIdAsync(int creditCardId)
        {
            return await _context.CreditCards.AsNoTracking()
                .FirstOrDefaultAsync(cc => cc.CreditCardId == creditCardId);
        }

        public async Task AddNewPaymentOptionAsync(CreditCard creditCard)
        {
            await _context.CreditCards.AddAsync(creditCard);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentOptionAsync(CreditCard creditCard)
        {
            _context.CreditCards.Update(creditCard);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePaymentOptionAsync(CreditCard creditCard)
        {
            _context.CreditCards.Remove(creditCard);
            await _context.SaveChangesAsync();
        }

        public async Task<CreditCard> GetPaymentOptionByPaymentMethodId(int paymentMethodId)
        {
            return await _context.CreditCards.AsNoTracking()
                .FirstOrDefaultAsync(cc => cc.PaymentMethodId == paymentMethodId);
        }
    }
}
