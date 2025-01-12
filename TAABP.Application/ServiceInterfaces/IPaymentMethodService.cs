using TAABP.Core.PaymentEntities;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IPaymentMethodService
    {
        Task<List<IPaymentOption>> GetAllUserPaymentOptionsAsync(string userId);
    }
}