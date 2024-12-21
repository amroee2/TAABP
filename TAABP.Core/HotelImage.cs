using System.ComponentModel.DataAnnotations;

namespace TAABP.Core
{
    public class HotelImage
    {
        [Key]
        public int HotelImageId { get; set; }
        [Required(ErrorMessage = "Please enter the image URL")]
        public string ImageUrl { get; set; }
        [Required]
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}
