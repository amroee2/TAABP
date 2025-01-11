using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.ReviewMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core;

namespace TAABP.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReviewMapper _reviewMapper;
        private readonly IHotelRepository _hotelRepository;
        private readonly IUserRepository _userRepository;
        public ReviewService(IReviewRepository reviewRepository, IReviewMapper reviewMapper,
            IHotelRepository hotelRepository, IUserRepository userRepository)
        {
            _reviewRepository = reviewRepository;
            _reviewMapper = reviewMapper;
            _hotelRepository = hotelRepository;
            _userRepository = userRepository;
        }

        public async Task<int> AddReviewAsync(ReviewDto reviewDto)
        {
            var user = await _userRepository.GetUserByIdAsync(reviewDto.UserId);
            var hotel = await _hotelRepository.GetHotelByIdAsync(reviewDto.HotelId);
            if(user == null || hotel == null)
            {
                throw new EntityNotFoundException();
            }
            var review = new Review();
            _reviewMapper.ReviewDtoToReview(reviewDto, review);
            await _reviewRepository.AddReviewAsync(review);
            await _reviewRepository.UpdateHotelRating(review.HotelId);
            return review.ReviewId;
        }

        public async Task UpdateReviewAsync(ReviewDto reviewDto)
        {
            try
            {
                Review review = await ValidateReviewAsync(reviewDto);
                _reviewMapper.ReviewDtoToReview(reviewDto, review);
                await _reviewRepository.UpdateReviewAsync(review);
                if(reviewDto.Rating != review.Rating)
                {
                    await _reviewRepository.UpdateHotelRating(review.HotelId);
                }
            }
            catch (EntityNotFoundException)
            {
                throw new EntityNotFoundException("Review Not Found");
            }
        }

        public async Task DeleteReviewAsync(string userId, int hotelId, int reviewId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null || review.UserId != userId || review.HotelId != hotelId)
            {
                throw new EntityNotFoundException("Review Not Found");
            }
            await _reviewRepository.DeleteReviewAsync(review);
            await _reviewRepository.UpdateHotelRating(hotelId);
        }

        public async Task<List<ReviewDto>> GetAllUserReviewsAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new EntityNotFoundException("User Not Found");
            }
            var reviews = await _reviewRepository.GetAllUserReviewsAsync(userId);
            return reviews.Select(review => _reviewMapper.ReviewToReviewDto(review)).ToList();
        }

        public async Task<List<ReviewDto>> GetAllHotelReviewsAsync(int hotelId)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(hotelId);
            if (hotel == null)
            {
                throw new EntityNotFoundException("Hotel Not Found");
            }
            var reviews = await _reviewRepository.GetAllHotelReviewsAsync(hotelId);
            return reviews.Select(review => _reviewMapper.ReviewToReviewDto(review)).ToList();
        }

        public async Task<ReviewDto> GetReviewByIdAsync(int reviewId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            try
            {
                await ValidateReviewAsync(_reviewMapper.ReviewToReviewDto(review));
            }
            catch (EntityNotFoundException)
            {
                throw new EntityNotFoundException("Review Not Found");
            }
            return _reviewMapper.ReviewToReviewDto(review);
        }

        private async Task<Review> ValidateReviewAsync(ReviewDto reviewDto)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewDto.ReviewId);
            var user = await _userRepository.GetUserByIdAsync(reviewDto.UserId);
            var hotel = await _hotelRepository.GetHotelByIdAsync(reviewDto.HotelId);
            if (user == null || hotel == null || review == null || review.HotelId != reviewDto.HotelId || review.UserId != reviewDto.UserId)
            {
                throw new EntityNotFoundException("Review Not Found");
            }
            return review;
        }
    }
}
