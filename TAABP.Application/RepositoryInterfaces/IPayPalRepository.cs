using TAABP.Core.PaymentEntities;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IPayPalRepository
    {
        Task<List<PayPal>> GetUserPaymentOptionAsync(string userId);
        Task<PayPal> GetPaymentOptionByIdAsync(int paymentOptionId);
        Task AddNewPaymentOptionAsync(PayPal paymentOption);
        Task UpdatePaymentOptionAsync(PayPal paymentOption);
        Task DeletePaymentOptionAsync(PayPal paymentOption);
    }
}
