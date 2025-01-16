using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.HotelMapping
{
    public interface IHotelMapper
    {
        void HotelDtoToHotel(HotelDto hotelDto, Hotel hotel);
        HotelDto HotelToHotelDto(Hotel hotel);
        void HotelImageDtoToHotelImage(HotelImageDto source, HotelImage target);
        HotelImageDto HotelImageToHotelImageDto(HotelImage hotelImage);
    }
}
