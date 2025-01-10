using TAABP.Core.PaymentEntities;

namespace TAABP.Application.DTOs
{
    public class CreditCardDto
    {
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string CVV { get; set; }
        public string CardHolderName { get; set; }
        public int PaymentMethodId { get; set; }
    }
}
