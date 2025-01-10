using Riok.Mapperly.Abstractions;
using TAABP.Application.DTOs;
using TAABP.Core.PaymentEntities;

namespace TAABP.Application.Profile.PayPalMapping
{
    [Mapper]
    public partial class PayPalMapper : IPayPalMapper
    {
        public partial void PayPalDtoToPayPal(PayPalDto payPalDto, PayPal payPal);
        public partial PayPalDto PayPalToPayPalDto(PayPal payPal);
    }
}
