using System.ComponentModel.DataAnnotations;

namespace TAABP.Core
{
    public class Amenity
    {
        [Key]
        public int AmenityId { get; set; }
        [Required(ErrorMessage = "Please enter the name of the amenity")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter a description of the amenity")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter the image URL")]
        public string ImageUrl { get; set; }
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}
