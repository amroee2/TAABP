namespace TAABP.Application.DTOs
{
    public class FeatueredDealDto
    {
        public int FeaturedDealId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int RoomId { get; set; }
    }
}
