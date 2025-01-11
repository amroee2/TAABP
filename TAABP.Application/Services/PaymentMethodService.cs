using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core.PaymentEntities;

namespace TAABP.Application.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly PaymentOptionServiceFactory _paymentOptionServiceFactory;

        public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository, PaymentOptionServiceFactory paymentOptionServiceFactory)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _paymentOptionServiceFactory = paymentOptionServiceFactory;
        }

        public async Task<List<IPaymentOption>> GetAllUserPaymentOptionsAsync(string userId)
        {
            var paymentMethods = await _paymentMethodRepository.GetUserPaymentMethodsAsync(userId);

            var paymentOptions = new List<IPaymentOption>();

            foreach (var paymentMethod in paymentMethods)
            {
                var service = _paymentOptionServiceFactory.GetService(paymentMethod.PaymentMethodName);
                var options = await service.GetPaymentOptionByPaymentMethodId(paymentMethod.PaymentMethodId);
                paymentOptions.Add(options);
            }

            return paymentOptions;
        }

    }
}
