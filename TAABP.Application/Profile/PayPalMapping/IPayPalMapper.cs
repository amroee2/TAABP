using TAABP.Application.DTOs;
using TAABP.Core.PaymentEntities;

namespace TAABP.Application.Profile.PayPalMapping
{
    public interface IPayPalMapper
    {
        public void PayPalDtoToPayPal(PayPalDto payPalDto, PayPal payPal);
        public PayPalDto PayPalToPayPalDto(PayPal payPal);
    }
}
