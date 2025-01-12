using TAABP.Application.DTOs;
using TAABP.Core.PaymentEntities;

namespace TAABP.Application.ServiceInterfaces
{
    public interface ICreditCardService
    {
        Task<CreditCard> GetPaymentOptionByIdAsync(int paymentOptionId);
        Task<int> AddNewPaymentOptionAsync(string userId, CreditCardDto paymentOption);
        Task UpdatePaymentOptionAsync(int creditCardId, string userId, CreditCardDto paymentOption);
        Task DeletePaymentOptionAsync(string userId, int paymentOptionId);
    }
}
