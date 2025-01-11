using TAABP.Core.PaymentEntities;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IPaymentOptionService
    {
        Task<IPaymentOption> GetPaymentOptionByPaymentMethodId(int paymentPmethodId);
    }
}