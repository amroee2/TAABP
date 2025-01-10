using System.ComponentModel.DataAnnotations;

namespace TAABP.Core.PaymentEntities
{
    public class CreditCard : IPaymentOption
    {
        [Key]
        public int CreditCardId { get; set; }
        [Required(ErrorMessage ="CardHolderName is required")]
        public string CardHolderName { get; set; }
        [Required(ErrorMessage = "CardNumber is required")]
        public string CardNumber { get; set; }
        [Required(ErrorMessage = "ExpirationDate is required")]
        public string ExpirationDate { get; set; }
        [Required(ErrorMessage = "CVV is required")]
        public string CVV { get; set; }
        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
