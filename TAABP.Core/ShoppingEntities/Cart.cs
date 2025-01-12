using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using TAABP.Core.PaymentEntities;

namespace TAABP.Core.ShoppingEntities
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public List<CartItem> CartItems { get; set; }
        public double TotalPrice { get; set; }
        public int? PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public CartStatus CartStatus { get; set; }
        public Cart()
        {

            CartItems = new List<CartItem>();
        }

        public void AddToPrice(double price)
        {
            TotalPrice += price;
        }

        public void RemoveFromPrice(double price)
        {
            TotalPrice -= price;
        }
    }
}
