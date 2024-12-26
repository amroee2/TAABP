using TAABP.Core;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IHotelRepository
    {
        Task CreateHotelAsync(Hotel hotel);
        Task<Hotel> GetHotelByIdAsync(int id);
        Task<List<Hotel>> GetHotelsAsync();
        Task DeleteHotelAsync(Hotel hotel);
        Task UpdateHotelAsync(Hotel hotel);
        Task AddNewImageAsync(HotelImage hotelImage);
        Task<HotelImage> GetHotelImageAsync(int imageId);
        Task<List<HotelImage>> GetHotelImagesAsync(int hotelId);
        Task DeleteHotelImageAsync(HotelImage hotelImage);
        Task UpdateHotelImageAsync(HotelImage hotelImage);
    }
}
