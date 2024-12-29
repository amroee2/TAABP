using Microsoft.EntityFrameworkCore;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core;

namespace TAABP.Infrastructure.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly TAABPDbContext _context;

        public CityRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public async Task<List<City>> GetCitiesAsync()
        {
            return await _context.Cities.AsNoTracking().ToListAsync();
        }

        public async Task<City> GetCityByIdAsync(int id)
        {
            return await _context.Cities.AsNoTracking().FirstOrDefaultAsync(c => c.CityId == id);
        }

        public async Task CreateCityAsync(City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCityAsync(City city)
        {
            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCityAsync(City city)
        {
            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
        }

        public async Task IncrementNumberOfHotelsAsync(int cityId)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.CityId == cityId);
            city!.NumberOfHotels++;
            await _context.SaveChangesAsync();
        }

        public async Task DecrementNumberOfHotelsAsync(int cityId)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.CityId == cityId);
            city!.NumberOfHotels--;
            await _context.SaveChangesAsync();
        }

        public async Task IncrementNumberOfVisitsAsync(int cityId)
        { 
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.CityId == cityId);
            city!.NumberOfVisits++;
            await _context.SaveChangesAsync();
        }

        public async Task DecrementNumberOfVisitsAsync(int cityId)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.CityId == cityId);
            city!.NumberOfVisits--;
            await _context.SaveChangesAsync();
        }
    }
}
