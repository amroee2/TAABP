using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IReviewService
    {
        Task<int> AddReviewAsync(ReviewDto reviewDto);
        Task UpdateReviewAsync(ReviewDto reviewDto);
        Task DeleteReviewAsync(string userId, int hotelId, int reviewId);
        Task<List<ReviewDto>> GetAllUserReviewsAsync(string userId);
        Task<List<ReviewDto>> GetAllHotelReviewsAsync(int hotelId);
        Task<ReviewDto> GetReviewByIdAsync(int reviewId);
    }
}
