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
        Task CreateNewHotelImageAsync(HotelImage hotelImage);
        Task<HotelImage> GetHotelImageByIdAsync(int hotelId, int imageId);
        Task<List<HotelImage>> GetHotelImagesAsync(int hotelId);
        Task DeleteHotelImageAsync(HotelImage hotelImage);
        Task UpdateHotelImageAsync(HotelImage hotelImage);
    }
}
