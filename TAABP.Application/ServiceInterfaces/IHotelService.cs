using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IHotelService
    {
        Task<int> CreateHotelAsync(int cityId, HotelDto hotelDto);
        Task<HotelDto> GetHotelByIdAsync(int id);
        Task<List<HotelDto>> GetHotelsAsync();
        Task DeleteHotelAsync(int id);
        Task UpdateHotelAsync(int Id, HotelDto hotelDto);
        Task AddNewImageAsync(int Id, HotelImageDto hotelImageDto);
        Task<HotelImageDto> GetHotelImageAsync( int imageId);
        Task<List<HotelImageDto>> GetHotelImagesAsync(int hotelId);
        Task DeleteHotelImageAsync(int imageId);
        Task UpdateHotelImageAsync(int imageId, HotelImageDto newHotelImage);
    }
}
