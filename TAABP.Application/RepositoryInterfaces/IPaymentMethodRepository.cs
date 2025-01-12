using TAABP.Core;
using TAABP.Core.PaymentEntities;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IPaymentMethodRepository
    {
        Task<List<PaymentMethod>> GetUserPaymentMethodsAsync(string userId);
        Task<PaymentMethod> GetPaymentMethodByIdAsync(int paymentMethodId);
        Task AddNewPaymentMethodAsync(PaymentMethod paymentMethod);
        Task DeletePaymentMethodAsync(PaymentMethod paymentMethod);
        Task<User> GetUserByPaymentMethodId(int paymentMethodId);
    }
}
