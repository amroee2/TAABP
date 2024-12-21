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
    }
}
