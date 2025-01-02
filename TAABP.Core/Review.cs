using System.ComponentModel.DataAnnotations;

namespace TAABP.Core
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
        [Required(ErrorMessage = "Please enter the review rating")]
        [StringLength(100)]
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }
}
