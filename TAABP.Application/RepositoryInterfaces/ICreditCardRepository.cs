using TAABP.Core.PaymentEntities;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface ICreditCardRepository
    {
        Task<List<CreditCard>> GetUserPaymentOptionAsync(string userId);
        Task<CreditCard> GetPaymentOptionByIdAsync(int creditCardId);
        Task AddNewPaymentOptionAsync(CreditCard creditCard);
        Task UpdatePaymentOptionAsync(CreditCard creditCard);
        Task DeletePaymentOptionAsync(CreditCard creditCard);
    }
}
