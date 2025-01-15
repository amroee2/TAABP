using TAABP.Core;

namespace TAABP.Application.DTOs
{
    public class HotelSearchResultDto
    {
        public int TotalResults { get; set; }
        public List<HotelDto> Hotels { get; set; }
    }
}
