using TAABP.Application.DTOs;
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
        Task IncrementNumberOfVisitsAsync(int hotelId);
        Task DecrementNumberOfVisitsAsync(int hotelId);
        Task CreateNewHotelImageAsync(HotelImage hotelImage);
        Task<HotelImage> GetHotelImageByIdAsync(int hotelId, int imageId);
        Task<List<HotelImage>> GetHotelImagesAsync(int hotelId);
        Task DeleteHotelImageAsync(HotelImage hotelImage);
        Task UpdateHotelImageAsync(HotelImage hotelImage);
        Task<List<Hotel>> GetFilteredHotelsAsync(FilterOptionsDto hotelFilter);
        Task IncrementNumberOfRoomsAsync(int hotelId);
        Task DecrementNumberOfRoomsAsync(int hotelId);
    }
}
