using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.HotelMapping
{
    public interface IHotelMapper
    {
        Hotel HotelDtoToHotel(HotelDto hotelDto);
        HotelDto HotelToHotelDto(Hotel hotel);
        void HotelImageDtoToHotelImage(HotelImageDto source, HotelImage target);
        HotelImageDto HotelImageToHotelImageDto(HotelImage hotelImage);
    }
}
