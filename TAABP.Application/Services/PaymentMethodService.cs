using TAABP.Application.Exceptions;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core.PaymentEntities;

namespace TAABP.Application.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly PaymentOptionServiceFactory _paymentOptionServiceFactory;
        private readonly IUserService _userService;
        public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository,
            PaymentOptionServiceFactory paymentOptionServiceFactory,
            IUserService userService)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _paymentOptionServiceFactory = paymentOptionServiceFactory;
            _userService = userService;
        }

        public async Task<List<IPaymentOption>> GetAllUserPaymentOptionsAsync(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new EntityNotFoundException("User not found");
            }
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
