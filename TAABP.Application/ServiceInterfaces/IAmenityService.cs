using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IAmenityService
    {
        Task<List<AmenityDto>> GetHotelAmenitiesAsync(int hotelId);
        Task<AmenityDto> GetAmenityByIdAsync(int hotelId, int amenityId);
        Task<int> CreateAmenityAsync(AmenityDto amenity);
        Task UpdateAmenityAsync(AmenityDto amenity);
        Task DeleteAmenityAsync(int hotelId, int roomId);
    }
}
