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
        public string UserId { get; set; }
        public User User { get; set; }
        public CartStatus CartStatus { get; set; }
        public Cart()
        {

            CartItems = new List<CartItem>();
        }
    }
}
