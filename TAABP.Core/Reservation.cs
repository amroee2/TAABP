using System.ComponentModel.DataAnnotations;

namespace TAABP.Core
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }
        [Required(ErrorMessage ="Start date is required")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public double Price { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
