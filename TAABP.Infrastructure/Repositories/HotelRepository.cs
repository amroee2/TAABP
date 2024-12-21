using Microsoft.EntityFrameworkCore;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core;

namespace TAABP.Infrastructure.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly TAABPDbContext _context;

        public HotelRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public async Task CreateHotelAsync(Hotel hotel)
        {
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task<Hotel> GetHotelAsync(int id)
        {
            return await _context.Hotels.AsNoTracking().FirstOrDefaultAsync(h => h.HotelId == id);
        }

        public async Task<List<Hotel>> GetHotelsAsync()
        {
            return await _context.Hotels.AsNoTracking().ToListAsync();
        }

        public async Task DeleteHotelAsync(Hotel hotel)
        {
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateHotelAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task AddNewImageAsync(HotelImage hotelImage)
        {
            _context.HotelImages.Add(hotelImage);
            await _context.SaveChangesAsync();
        }
    }
}
