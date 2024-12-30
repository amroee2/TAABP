namespace TAABP.Application.DTOs
{
    public class FilterOptionsDto
    {
        public double[] PriceRange { get; set; }
        public int[] StarRating { get; set; }
        public string[] Amenities { get; set; }
        public string RoomType { get; set; }
    }
}
