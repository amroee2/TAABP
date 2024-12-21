using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.HotelMapping
{
    public interface IHotelMapper
    {
        Hotel HotelDtoToUser(HotelDto hotelDto);
    }
}
