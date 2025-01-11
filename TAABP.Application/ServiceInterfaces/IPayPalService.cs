using TAABP.Application.DTOs;
using TAABP.Core.PaymentEntities;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IPayPalService
    {
        Task<PayPal> GetPaymentOptionByIdAsync(int paymentOptionId);
        Task<int> AddNewPaymentOptionAsync(string userId, PayPalDto paymentOption);
        Task UpdatePaymentOptionAsync(int paypalid, string userId, PayPalDto paymentOption);
        Task DeletePaymentOptionAsync(string userId, int PayPalId);
    }
}
