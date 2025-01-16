using System.ComponentModel.DataAnnotations;

namespace TAABP.Core
{
    public class FeaturedDeal
    {
        [Key]
        public int FeaturedDealId { get; set; }
        [Required(ErrorMessage ="Please enter a title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter a description")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter a discount")]
        public double Discount { get; set; }
        [Required(ErrorMessage = "Please enter a start date")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Please enter an end date")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Please enter a if the deal availability")]
        public bool IsActive { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
