using Microsoft.EntityFrameworkCore;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core;

namespace TAABP.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly TAABPDbContext _context;

        public ReviewRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public Task<List<Review>> GetAllUserReviewsAsync(string userId)
        {
            return _context.Reviews.AsNoTracking()
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public Task<List<Review>> GetAllHotelReviewsAsync(int hotelId)
        {
            return _context.Reviews.AsNoTracking()
                .Where(r => r.HotelId == hotelId)
                .ToListAsync();
        }

        public Task<Review> GetReviewByIdAsync(int reviewId)
        {
            return _context.Reviews.AsNoTracking()
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        }

        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReviewAsync(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateHotelRating(int hotelId)
        {
            var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == hotelId);
            var reviews = await _context.Reviews.Where(r => r.HotelId == hotelId).ToListAsync();
            hotel.Rating =  reviews.Count > 0 ? (int) reviews.Average(r => r.Rating) : 0;
            await _context.SaveChangesAsync();
        }
    }
}
