using TAABP.Core.PaymentEntities;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface ICreditCardRepository
    {
        Task<CreditCard> GetPaymentOptionByIdAsync(int creditCardId);
        Task AddNewPaymentOptionAsync(CreditCard creditCard);
        Task UpdatePaymentOptionAsync(CreditCard creditCard);
        Task<CreditCard> GetPaymentOptionByPaymentMethodId(int paymentMethodId);
    }
}
