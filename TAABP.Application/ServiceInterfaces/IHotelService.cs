using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IHotelService
    {
        Task CreateHotelAsync( HotelDto hotelDto);
    }
}
