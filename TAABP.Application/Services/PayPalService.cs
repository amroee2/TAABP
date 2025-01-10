using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.PayPalMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core;
using TAABP.Core.PaymentEntities;

namespace TAABP.Application.Services
{
    public class PayPalService : IPayPalService, IPaymentOptionService
    {
        private readonly IPayPalRepository _payPalRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IPayPalMapper _payPalMapper;
        public PayPalService(IPayPalRepository payPalRepository, IUserRepository userRepository,
            IPaymentMethodRepository paymentMethodRepository, IPayPalMapper payPalMapper)
        {
            _payPalRepository = payPalRepository;
            _userRepository = userRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _payPalMapper = payPalMapper;
        }

        public async Task<List<IPaymentOption>> GetUserPaymentOptionAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new EntityNotFoundException("User not found");
            }
            var payPals = await _payPalRepository.GetUserPaymentOptionAsync(userId);
            return payPals.Cast<IPaymentOption>().ToList();
        }

        public async Task<PayPal> GetPaymentOptionByIdAsync(int paymentOptionId)
        {
            var payPal = await _payPalRepository.GetPaymentOptionByIdAsync(paymentOptionId);
            if (payPal == null)
            {
                throw new EntityNotFoundException("PayPal account not found");
            }
            return payPal;
        }

        public async Task<int> AddNewPaymentOptionAsync(string userId, PayPalDto paymentOption)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new EntityNotFoundException("User not found");
            }
            PaymentMethod paymentMethod = new PaymentMethod
            {
                UserId = userId,
                PaymentMethodName = PaymentMethodOption.PayPal
            };
            await _paymentMethodRepository.AddNewPaymentMethodAsync(paymentMethod);
            var payPal = new PayPal();
            _payPalMapper.PayPalDtoToPayPal(paymentOption, payPal);
            payPal.PaymentMethodId = paymentMethod.PaymentMethodId;
            await _payPalRepository.AddNewPaymentOptionAsync(payPal);
            return payPal.PayPalId;
        }

        public async Task UpdatePaymentOptionAsync(int payPalId, string userId, PayPalDto paymentOption)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new EntityNotFoundException("User not found");
            }
            var card = await _payPalRepository.GetPaymentOptionByIdAsync(payPalId);
            if (card == null)
            {
                throw new EntityNotFoundException("Credit card not found");
            }
            var paymentMethod = await _paymentMethodRepository.GetPaymentMethodByIdAsync(card.PaymentMethodId);

            if (paymentMethod == null)
            {
                throw new EntityNotFoundException("Payment method not found");
            }
            if (paymentMethod.UserId != userId)
            {
                throw new EntityNotFoundException("Payment method does not belong to user");
            }
            paymentOption.PaymentMethodId = card.PaymentMethodId;
            _payPalMapper.PayPalDtoToPayPal(paymentOption, card);
            await _payPalRepository.UpdatePaymentOptionAsync(card);
        }

        public async Task DeletePaymentOptionAsync(string userId, int paymentOptionId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new EntityNotFoundException("User not found");
            }

            var payPal = await _payPalRepository.GetPaymentOptionByIdAsync(paymentOptionId);
            if (payPal == null)
            {
                throw new EntityNotFoundException("PayPal account not found");
            }
            var paymentMethod = await _paymentMethodRepository.GetPaymentMethodByIdAsync(payPal.PaymentMethodId);
            if (paymentMethod == null)
            {
                throw new EntityNotFoundException("Payment method not found");
            }
            if (paymentMethod.UserId != userId)
            {
                throw new EntityNotFoundException("Payment method not found");
            }
            await _paymentMethodRepository.DeletePaymentMethodAsync(paymentMethod);
        }
    }
}
