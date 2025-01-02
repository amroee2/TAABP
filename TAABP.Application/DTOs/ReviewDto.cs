namespace TAABP.Application.DTOs
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public string? UserId { get; set; }
        public int HotelId { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }

        public ReviewDto()
        {
            Date = DateTime.Now;
        }
    }
}
