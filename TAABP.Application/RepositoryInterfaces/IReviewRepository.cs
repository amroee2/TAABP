using TAABP.Core;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllUserReviewsAsync(string userId);
        Task<List<Review>> GetAllHotelReviewsAsync(int hotelId);
        Task<Review> GetReviewByIdAsync(int reviewId);
        Task AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(Review review);
    }
}
