using TAABP.Application.DTOs;
using TAABP.Core.PaymentEntities;

namespace TAABP.Application.Profile.CreditCardMapping
{
    public interface ICreditCardMapper
    {
        void CreditCardDtoToCreditCard(CreditCardDto creditCardDto, CreditCard creditCard);
        CreditCardDto CreditCardToCreditCardDto(CreditCard creditCard);
    }
}
