using System.ComponentModel.DataAnnotations;

namespace TAABP.Application.DTOs
{
    public class HotelDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string Owner { get; set; }
        public int Rating { get; set; }
        public int NumberOfRooms { get; set; }
    }
}
