using TAABP.Core.PaymentEntities;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IPayPalRepository
    {
        Task<PayPal> GetPaymentOptionByIdAsync(int paymentOptionId);
        Task AddNewPaymentOptionAsync(PayPal paymentOption);
        Task UpdatePaymentOptionAsync(PayPal paymentOption);
        Task<PayPal> GetPaymentOptionByPaymentMethodId(int paymentMethodId);
        Task<bool> CheckIfEmailAlreadyExists(string email);
    }
}
