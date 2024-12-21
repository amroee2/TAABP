using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IHotelService
    {
        Task CreateHotelAsync( HotelDto hotelDto);
        Task<HotelDto> GetHotelAsync(int id);
        Task<List<HotelDto>> GetHotelsAsync();
        Task DeleteHotelAsync(int id);
        Task UpdateHotelAsync(int Id, HotelDto hotelDto);
        Task AddNewImageAsync(int Id, HotelImageDto hotelImageDto);
        Task<HotelImageDto> GetHotelImage(int hotelId, int imageId);
        Task<List<HotelImageDto>> GetHotelImages(int hotelId);
    }
}
