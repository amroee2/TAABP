using TAABP.Core;

namespace TAABP.Application.DTOs
{
    public class RoomDto
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public int AdultsCapacity { get; set; }
        public int ChildrenCapacity { get; set; }
        public int HotelId { get; set; }
        public double PricePerNight { get; set; }
        public bool IsAvailable { get; set; }
        public int Number { get; set; }
        public RoomType Type { get; set; }
    }
}
