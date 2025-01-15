using TAABP.Core.PaymentEntities;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IPaymentOptionServiceFactory
    {
        IPaymentOptionService GetService(PaymentMethodOption paymentType);
    }
}
