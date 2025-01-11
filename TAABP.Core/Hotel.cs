using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace TAABP.Core
{
    public class Hotel
    {
        [Key]
        public int HotelId { get; set; }
        [Required(ErrorMessage = "Please enter the hotel name")]
        [Display(Name = "Hotel Name")]
        [StringLength(30)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter the hotel address")]
        [Display(Name = "Address")]
        [StringLength(100)]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please enter the hotel phone number")]
        [Display(Name = "Phone Number")]
        [StringLength(30)]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter the hotel description")]
        [Display(Name = "Description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter the hotel thumbnail")]
        [Display(Name = "Thumbnail")]
        [StringLength(100)]
        public string Thumbnail { get; set; }
        [Required(ErrorMessage = "Please enter the hotel owner")]
        [Display(Name = "Owner")]
        [StringLength(30)]
        public string Owner { get; set; }
        [Required(ErrorMessage = "Please enter the hotel rating")]
        [Display(Name = "Rating")]
        public int Rating { get; set; }
        [Required(ErrorMessage = "Please enter the hotel number of rooms")]
        [Display(Name = "Number of Rooms")]
        public int NumberOfRooms { get; set; }
        public int NumberOfVisits { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public List<HotelImage> HotelImages { get; set; }
        public List<Amenity> Amenities { get; set; }
        public List<Room> Rooms { get; set; }
        public List<Review> Reviews { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public Hotel()
        {
            HotelImages = new List<HotelImage>();
            Amenities = new List<Amenity>();
            Rooms = new List<Room>();
            Reviews = new List<Review>();
        }
    }
}
