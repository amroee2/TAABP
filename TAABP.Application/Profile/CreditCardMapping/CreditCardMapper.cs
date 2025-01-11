using Riok.Mapperly.Abstractions;
using TAABP.Application.DTOs;
using TAABP.Core.PaymentEntities;

namespace TAABP.Application.Profile.CreditCardMapping
{
    [Mapper]
    public partial class CreditCardMapper : ICreditCardMapper
    {
        public partial void CreditCardDtoToCreditCard(CreditCardDto creditCardDto, CreditCard creditCard);
        public partial CreditCardDto CreditCardToCreditCardDto(CreditCard creditCard);
    }
}
