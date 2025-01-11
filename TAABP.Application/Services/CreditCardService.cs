using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.CreditCardMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core.PaymentEntities;

namespace TAABP.Application.Services
{
    public class CreditCardService : ICreditCardService, IPaymentOptionService
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ICreditCardMapper _creditCardMapper;
        public CreditCardService(ICreditCardRepository creditCardRepository, IUserRepository userRepository,
            IPaymentMethodRepository paymentMethodRepository, ICreditCardMapper creditCardMapper)
        {
            _creditCardRepository = creditCardRepository;
            _userRepository = userRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _creditCardMapper = creditCardMapper;
        }

        public async Task<IPaymentOption> GetPaymentOptionByPaymentMethodId(int paymentMethodId)
        {
            var creditCard = await _creditCardRepository.GetPaymentOptionByPaymentMethodId(paymentMethodId);
            return creditCard;
        }
        public async Task<CreditCard> GetPaymentOptionByIdAsync(int paymentOptionId)
        {
            var creditCard= await _creditCardRepository.GetPaymentOptionByIdAsync(paymentOptionId);
            if (creditCard == null)
            {
                throw new EntityNotFoundException("Credit card not found");
            }
            return creditCard;
        }

        public async Task<int> AddNewPaymentOptionAsync(string userId, CreditCardDto paymentOption)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if(user == null)
            {
                throw new EntityNotFoundException("User not found");
            }
            PaymentMethod paymentMethod = new PaymentMethod
            {
                UserId = userId,
                PaymentMethodName = PaymentMethodOption.CreditCard
            };
            await _paymentMethodRepository.AddNewPaymentMethodAsync(paymentMethod);
            var creditCard = new CreditCard();
            _creditCardMapper.CreditCardDtoToCreditCard(paymentOption, creditCard);
            creditCard.PaymentMethodId = paymentMethod.PaymentMethodId;
            await _creditCardRepository.AddNewPaymentOptionAsync(creditCard);
            return creditCard.CreditCardId;
        }

        public async Task UpdatePaymentOptionAsync(int creditCardId, string userID, CreditCardDto paymentOption)
        {
            var user = await _userRepository.GetUserByIdAsync(userID);
            if (user == null)
            {
                throw new EntityNotFoundException("User not found");
            }
            var card = await _creditCardRepository.GetPaymentOptionByIdAsync(creditCardId);
            if (card == null)
            {
                throw new EntityNotFoundException("Credit card not found");
            }
            var paymentMethod = await _paymentMethodRepository.GetPaymentMethodByIdAsync(card.PaymentMethodId);

            if (paymentMethod == null)
            {
                throw new EntityNotFoundException("Payment method not found");
            }
            if(paymentMethod.UserId != userID)
            {
                throw new EntityNotFoundException("Payment method does not belong to user");
            }
            paymentOption.PaymentMethodId = card.PaymentMethodId;
            _creditCardMapper.CreditCardDtoToCreditCard(paymentOption, card);
            await _creditCardRepository.UpdatePaymentOptionAsync(card);
        }

        public async Task DeletePaymentOptionAsync(string userId, int paymentOptionId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new EntityNotFoundException("User not found");
            }
            var card = await _creditCardRepository.GetPaymentOptionByIdAsync(paymentOptionId);
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
            await _paymentMethodRepository.DeletePaymentMethodAsync(paymentMethod);
        }
    }
}
