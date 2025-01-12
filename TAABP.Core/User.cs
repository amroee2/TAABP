using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TAABP.Core.PaymentEntities;

namespace TAABP.Core
{
    public class User: IdentityUser
    {
        [Required(ErrorMessage = "Please enter your first Name")]
        [Display(Name = "First Name")]
        [StringLength(30)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last Name")]
        [Display(Name = "Last Name")]
        [StringLength(30)]
        public string LastName { get; set; }

        [Display(Name = "Address")]
        [StringLength(100)]
        public string? Address { get; set; }
        public List<Reservation> Reservations { get; set; }
        public List<Review> Reviews { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public User()
        {
            Reservations = new List<Reservation>();
            Reviews = new List<Review>();
            PaymentMethods = new List<PaymentMethod>();
        }
    }
}
