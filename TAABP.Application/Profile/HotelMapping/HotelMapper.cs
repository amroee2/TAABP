using Riok.Mapperly.Abstractions;
using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.HotelMapping
{
    [Mapper]
    public partial class HotelMapper : IHotelMapper
    {
        public partial HotelDto HotelToHotelDto(Hotel hotel);
        public partial Hotel HotelDtoToHotel(HotelDto hotelDto);
    }
}
