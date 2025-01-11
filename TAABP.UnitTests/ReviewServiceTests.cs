using AutoFixture;
using Moq;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.ReviewMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.Services;
using TAABP.Core;

namespace TAABP.UnitTests
{
    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IHotelRepository> _hotelRepositoryMock;
        private readonly Mock<IReviewMapper> _reviewMapperMock;
        private readonly ReviewService _reviewService;
        private readonly Fixture _fixture;

        public ReviewServiceTests()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _hotelRepositoryMock = new Mock<IHotelRepository>();
            _reviewMapperMock = new Mock<IReviewMapper>();
            _reviewService = new ReviewService(
                _reviewRepositoryMock.Object,
                _reviewMapperMock.Object,
                _hotelRepositoryMock.Object,
                _userRepositoryMock.Object);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task AddReviewAsync_ShouldAddReview_WhenUserAndHotelExist()
        {
            // Arrange
            var reviewDto = _fixture.Create<ReviewDto>();
            var user = _fixture.Create<User>();
            var hotel = _fixture.Create<Hotel>();
            var review = new Review();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(reviewDto.UserId)).ReturnsAsync(user);
            _hotelRepositoryMock.Setup(repo => repo.GetHotelByIdAsync(reviewDto.HotelId)).ReturnsAsync(hotel);
            _reviewMapperMock.Setup(mapper => mapper.ReviewDtoToReview(reviewDto, It.IsAny<Review>()))
                             .Callback<ReviewDto, Review>((dto, r) => { r.ReviewId = 1; });

            // Act
            var result = await _reviewService.AddReviewAsync(reviewDto);

            // Assert
            Assert.Equal(1, result);
            _reviewRepositoryMock.Verify(repo => repo.AddReviewAsync(It.IsAny<Review>()), Times.Once);
        }

        [Fact]
        public async Task AddReviewAsync_ShouldThrowException_WhenUserOrHotelDoesNotExist()
        {
            // Arrange
            var reviewDto = _fixture.Create<ReviewDto>();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(reviewDto.UserId)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _reviewService.AddReviewAsync(reviewDto));
        }

        [Fact]
        public async Task DeleteReviewAsync_ShouldDeleteReview_WhenReviewExists()
        {
            // Arrange
            var review = _fixture.Create<Review>();
            review.UserId = "testUser";
            review.HotelId = 1;

            _reviewRepositoryMock.Setup(repo => repo.GetReviewByIdAsync(review.ReviewId)).ReturnsAsync(review);

            // Act
            await _reviewService.DeleteReviewAsync("testUser", 1, review.ReviewId);

            // Assert
            _reviewRepositoryMock.Verify(repo => repo.DeleteReviewAsync(review), Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_ShouldThrowException_WhenReviewDoesNotExist()
        {
            // Arrange
            _reviewRepositoryMock.Setup(repo => repo.GetReviewByIdAsync(It.IsAny<int>())).ReturnsAsync((Review)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _reviewService.DeleteReviewAsync("testUser", 1, 123));
        }

        [Fact]
        public async Task GetAllUserReviewsAsync_ShouldReturnReviews_WhenUserExists()
        {
            // Arrange
            var userId = "testUser";
            var reviews = _fixture.CreateMany<Review>().ToList();
            var reviewDtos = reviews.Select(r => _fixture.Create<ReviewDto>()).ToList();

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(new User());
            _reviewRepositoryMock.Setup(repo => repo.GetAllUserReviewsAsync(userId)).ReturnsAsync(reviews);
            _reviewMapperMock.Setup(mapper => mapper.ReviewToReviewDto(It.IsAny<Review>()))
                             .Returns((Review r) => reviewDtos.First());

            // Act
            var result = await _reviewService.GetAllUserReviewsAsync(userId);

            // Assert
            Assert.Equal(reviewDtos.Count, result.Count);
        }

        [Fact]
        public async Task GetAllHotelReviewsAsync_ShouldReturnReviews_WhenHotelExists()
        {
            // Arrange
            var hotelId = 1;
            var reviews = _fixture.CreateMany<Review>().ToList();
            var reviewDtos = reviews.Select(r => _fixture.Create<ReviewDto>()).ToList();

            _hotelRepositoryMock.Setup(repo => repo.GetHotelByIdAsync(hotelId)).ReturnsAsync(new Hotel());
            _reviewRepositoryMock.Setup(repo => repo.GetAllHotelReviewsAsync(hotelId)).ReturnsAsync(reviews);
            _reviewMapperMock.Setup(mapper => mapper.ReviewToReviewDto(It.IsAny<Review>()))
                             .Returns((Review r) => reviewDtos.First());

            // Act
            var result = await _reviewService.GetAllHotelReviewsAsync(hotelId);

            // Assert
            Assert.Equal(reviewDtos.Count, result.Count);
        }

        [Fact]
        public async Task UpdateReviewAsync_ShouldUpdateReview_WhenReviewIsValid()
        {
            // Arrange
            var reviewDto = _fixture.Create<ReviewDto>();
            var review = new Review
            {
                ReviewId = reviewDto.ReviewId,
                UserId = reviewDto.UserId,
                HotelId = reviewDto.HotelId
            };

            _reviewRepositoryMock.Setup(repo => repo.GetReviewByIdAsync(reviewDto.ReviewId))
                                 .ReturnsAsync(review);
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(reviewDto.UserId))
                               .ReturnsAsync(new User());
            _hotelRepositoryMock.Setup(repo => repo.GetHotelByIdAsync(reviewDto.HotelId))
                                .ReturnsAsync(new Hotel());
            _reviewMapperMock.Setup(mapper => mapper.ReviewDtoToReview(reviewDto, review))
                             .Callback<ReviewDto, Review>((dto, r) =>
                             {
                                 r.Comment = dto.Comment;
                                 r.Date = dto.Date;
                             });

            // Act
            await _reviewService.UpdateReviewAsync(reviewDto);

            // Assert
            _reviewMapperMock.Verify(mapper => mapper.ReviewDtoToReview(reviewDto, review), Times.Once);
            _reviewRepositoryMock.Verify(repo => repo.UpdateReviewAsync(review), Times.Once);
        }


        [Fact]
        public async Task UpdateReviewAsync_ShouldThrowException_WhenReviewIsInvalid()
        {
            // Arrange
            var reviewDto = _fixture.Create<ReviewDto>();

            _reviewRepositoryMock.Setup(repo => repo.GetReviewByIdAsync(reviewDto.ReviewId)).ReturnsAsync((Review)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _reviewService.UpdateReviewAsync(reviewDto));
        }
    }
}
