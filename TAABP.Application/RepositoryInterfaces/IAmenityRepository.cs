using TAABP.Core;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IAmenityRepository
    {
        Task<List<Amenity>> GetHotelAmenitiesAsync(int hotelId);
        Task<Amenity> GetAmenityAsync(int amenityId);
        Task<Amenity> CreateAmenityAsync(Amenity amenity);
        Task<Amenity> UpdateAmenityAsync(Amenity amenity);
        Task DeleteAmenityAsync(Amenity amenity);
    }
}
