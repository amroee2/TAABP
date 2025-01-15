using TAABP.Core;
using TAABP.Core.PaymentEntities;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IPaymentMethodService
    {
        Task<List<IPaymentOption>> GetAllUserPaymentOptionsAsync(string userId);
        Task<User> GetUserByPaymentMethodId(int paymentMethodId);
    }
}