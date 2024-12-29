using System.ComponentModel.DataAnnotations;

namespace TAABP.Core
{
    public class City
    {
        [Key]
        public int CityId { get; set; }
        [Required(ErrorMessage = "Please enter the city name")]
        [Display(Name = "City Name")]
        [StringLength(30)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter the country")]
        [Display(Name = "Country")]
        [StringLength(30)]
        public string Country { get; set; }
        [Required(ErrorMessage = "Please enter the city thumbnail")]
        [Display(Name = "Thumbnail")]
        [StringLength(100)]
        public string Thumbnail { get; set; }
        [Required(ErrorMessage = "Please enter the city description")]
        [Display(Name = "Description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter the city post office")]
        [Display(Name = "Post Office")]
        [StringLength(30)]
        public string PostOffice { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Number of Hotels must be greater than or equal to 0.")]
        public int NumberOfHotels { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Number of visits must be greater than or equal to 0.")]
        public int NumberOfVisits { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public List<Hotel> Hotels { get; set; }

        public City()
        {
            Hotels = new List<Hotel>();
        }
    }
}
