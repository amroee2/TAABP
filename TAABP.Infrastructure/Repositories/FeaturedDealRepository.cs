using Microsoft.EntityFrameworkCore;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core;

namespace TAABP.Infrastructure.Repositories
{
    public class FeaturedDealRepository : IFeaturedDealRepository
    {
        private readonly TAABPDbContext _context;

        public FeaturedDealRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public async Task<FeaturedDeal> GetFeaturedDealAsync(int id)
        {
            return await _context.FeaturedDeals.AsNoTracking().FirstOrDefaultAsync(fd => fd.FeaturedDealId == id);
        }

        public async Task<List<FeaturedDeal>> GetFeaturedDealsAsync()
        {
            return await _context.FeaturedDeals.AsNoTracking().ToListAsync();
        }

        public async Task CreateFeaturedDealAsync(FeaturedDeal featuredDeal)
        {
            _context.FeaturedDeals.Add(featuredDeal);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFeaturedDealAsync(FeaturedDeal featuredDeal)
        {
            _context.Update(featuredDeal);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFeaturedDealAsync(FeaturedDeal featuredDeal)
        {
            _context.FeaturedDeals.Remove(featuredDeal);
            await _context.SaveChangesAsync();
        }

        public async Task<FeaturedDeal> GetActiveFeaturedDealByRoomIdAsync(int roomId)
        {
            return await _context.FeaturedDeals.AsNoTracking().FirstOrDefaultAsync(fd => fd.RoomId == roomId && fd.IsActive);
        }
    }
}
