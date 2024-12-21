using TAABP.Core;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IHotelRepository
    {
        Task CreateHotelAsync(Hotel hotel);
        Task<Hotel> GetHotelAsync(int id);
        Task<List<Hotel>> GetHotelsAsync();
        Task DeleteHotelAsync(Hotel hotel);
        Task UpdateHotelAsync(Hotel hotel);
        Task AddNewImageAsync(HotelImage hotelImage);
        Task<HotelImage> GetHotelImage(int hotelId, int imageId);
        Task<List<HotelImage>> GetHotelImages(int hotelId);
    }
}
