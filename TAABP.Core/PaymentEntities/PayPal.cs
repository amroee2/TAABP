using System.ComponentModel.DataAnnotations;

namespace TAABP.Core.PaymentEntities
{
    public class PayPal : IPaymentOption
    {
        [Key]
        public int PayPalId { get; set; }
        public int PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        [Required(ErrorMessage = "PayPalEmail is required")]
        public string PayPalEmail { get; set; }
    }
}
