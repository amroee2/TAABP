using System.ComponentModel.DataAnnotations;
using TAABP.Core.ShoppingEntities;

namespace TAABP.Core
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }
        [Required(ErrorMessage = "Please enter the room name")]
        [Display(Name = "Room Name")]
        [StringLength(30)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter the room description")]
        [Display(Name = "Description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter the room thumbnail")]
        [Display(Name = "Thumbnail")]
        [StringLength(100)]
        public string Thumbnail { get; set; }
        [Required(ErrorMessage = "Please enter the room adults capacity")]
        [Display(Name = "Adults Capacity")]
        public int AdultsCapacity { get; set; }
        [Required(ErrorMessage = "Please enter the room children capacity")]
        [Display(Name = "Children Capacity")]
        public int ChildrenCapacity { get; set; }
        [Required(ErrorMessage = "Please enter the room price per night")]
        [Display(Name = "Price Per Night")]
        public double PricePerNight { get; set; }
        [Required(ErrorMessage = "Please enter the room status")]
        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }
        [Required(ErrorMessage = "Please enter the room number")]
        [Display(Name = "Room Number")]
        public int RoomNumber { get; set; }
        [Required(ErrorMessage = "Please enter the room type")]
        [Display(Name = "Room Type")]
        public RoomType Type { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
        public List<RoomImage> RoomImages { get; set; }
        public List<FeaturedDeal> FeaturedDeals { get; set; }
        public List<Reservation> Reservations { get; set; }
        public List<CartItem> CartItems { get; set; }
        public Room()
        {
            RoomImages = new List<RoomImage>();
            FeaturedDeals = new List<FeaturedDeal>();
            Reservations = new List<Reservation>();
            CartItems = new List<CartItem>();
        }
    }
}
