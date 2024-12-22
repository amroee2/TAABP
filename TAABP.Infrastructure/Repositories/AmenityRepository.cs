using Microsoft.EntityFrameworkCore;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core;

namespace TAABP.Infrastructure.Repositories
{
    public class AmenityRepository : IAmenityRepository
    {
        private readonly TAABPDbContext _context;

        public AmenityRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public async Task<List<Amenity>> GetHotelAmenitiesAsync(int hotelId)
        {
            return await _context.Amenities
                .Where(a => a.HotelId == hotelId)
                .ToListAsync();
        }

        public async Task<Amenity> GetAmenityAsync(int amenityId)
        {
            return await _context.Amenities
                .FirstOrDefaultAsync(a => a.AmenityId == amenityId);
        }

        public async Task<Amenity> CreateAmenityAsync(Amenity amenity)
        {
            _context.Amenities.Add(amenity);
            await _context.SaveChangesAsync();
            return amenity;
        }

        public async Task<Amenity> UpdateAmenityAsync(Amenity amenity)
        {
            _context.Amenities.Update(amenity);
            await _context.SaveChangesAsync();
            return amenity;
        }

        public async Task DeleteAmenityAsync(Amenity amenity)
        {
            _context.Amenities.Remove(amenity);
            await _context.SaveChangesAsync();
        }
    }
}
