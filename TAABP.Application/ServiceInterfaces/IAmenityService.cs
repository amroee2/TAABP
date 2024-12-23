using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IAmenityService
    {
        Task<List<AmenityDto>> GetHotelAmenitiesAsync(int hotelId);
        Task<AmenityDto> GetAmenityAsync(int amenityId);
        Task CreateAmenityAsync(AmenityDto amenity);
        Task UpdateAmenityAsync(AmenityDto amenity);
        Task DeleteAmenityAsync(AmenityDto amenity);
    }
}
