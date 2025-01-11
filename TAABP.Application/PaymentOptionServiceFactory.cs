using TAABP.Application.ServiceInterfaces;
using TAABP.Application.Services;
using TAABP.Core.PaymentEntities;

namespace TAABP.Application
{
    public class PaymentOptionServiceFactory
    {
        private readonly IEnumerable<IPaymentOptionService> _services;

        public PaymentOptionServiceFactory(IEnumerable<IPaymentOptionService> services)
        {
            _services = services;
        }

        public IPaymentOptionService GetService(PaymentMethodOption paymentType)
        {
            return paymentType switch
            {
                PaymentMethodOption.CreditCard => _services.OfType<CreditCardService>().FirstOrDefault(),
                PaymentMethodOption.PayPal => _services.OfType<PayPalService>().FirstOrDefault(),
                _ => throw new ArgumentException("Unsupported payment type")
            };
        }
    }
}
