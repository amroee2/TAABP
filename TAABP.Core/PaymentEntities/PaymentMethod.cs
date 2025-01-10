using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TAABP.Core.PaymentEntities
{
    public class PaymentMethod
    {
        [Key]
        public int PaymentMethodId { get; set; }
        [Required(ErrorMessage = "Payment method is required")]
        public PaymentMethodOption PaymentMethodName { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

    }
}
