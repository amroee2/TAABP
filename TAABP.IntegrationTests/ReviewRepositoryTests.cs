using AutoFixture;
using TAABP.Infrastructure.Repositories;
using TAABP.Infrastructure;
using Microsoft.EntityFrameworkCore;
using TAABP.Core;

namespace TAABP.IntegrationTests
{
    public class ReviewRepositoryTests
    {
        private readonly TAABPDbContext _context;
        private readonly ReviewRepository _reviewRepository;
        private readonly IFixture _fixture;

        public ReviewRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TAABPDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TAABPDbContext(options);

            _reviewRepository = new ReviewRepository(_context);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task CreateReviewAsync_ShouldAddReviewAsync()
        {
            // Arrange
            var review = _fixture.Create<Review>();

            // Act
            await _reviewRepository.AddReviewAsync(review);

            // Assert
            var result = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == review.ReviewId);
            Assert.Equal(review, result);
        }

        [Fact]
        public async Task GetReviewByIdAsync_ShouldReturnReviewByIdAsync()
        {
            // Arrange
            var review = _fixture.Create<Review>();
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            // Act
            var result = await _reviewRepository.GetReviewByIdAsync(review.ReviewId);

            // Assert
            Assert.Equal(review.ReviewId, result.ReviewId);
        }

        [Fact]
        public async Task GetReviewsAsync_ShouldReturnAllReviewsAsync()
        {
            // Arrange
            var user = _fixture.Create<User>();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            _fixture.Customize<Review>(r => r.With(h => h.UserId, user.Id));
            var reviews = _fixture.CreateMany<Review>().ToList();
            await _context.Reviews.AddRangeAsync(reviews);
            await _context.SaveChangesAsync();

            // Act
            var result = await _reviewRepository.GetAllUserReviewsAsync(user.Id);

            // Assert
            Assert.Equal(reviews.Count, result.Count());
        }

        [Fact]
        public async Task GetHotelReviewsAsync_ShouldReturnAllHotelReviewsAsync()
        {
            // Arrange
            var hotel = _fixture.Create<Hotel>();
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();
            _fixture.Customize<Review>(r => r.With(h => h.HotelId, hotel.HotelId));
            var reviews = _fixture.CreateMany<Review>().ToList();
            await _context.Reviews.AddRangeAsync(reviews);
            await _context.SaveChangesAsync();

            // Act
            var result = await _reviewRepository.GetAllHotelReviewsAsync(hotel.HotelId);

            // Assert
            Assert.Equal(reviews.Count, result.Count());
        }

        [Fact]
        public async Task UpdateReviewAsync_ShouldUpdateReviewAsync()
        {
            // Arrange
            var review = _fixture.Create<Review>();
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            review.Comment = "comment";

            // Act
            await _reviewRepository.UpdateReviewAsync(review);

            // Assert
            var result = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == review.ReviewId);
            Assert.Equal(review.Comment, result.Comment);
        }

        [Fact]
        public async Task DeleteReviewAsync_ShouldDeleteReviewAsync()
        {
            // Arrange
            var review = _fixture.Create<Review>();
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            // Act
            await _reviewRepository.DeleteReviewAsync(review);

            // Assert
            var result = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == review.ReviewId);
            Assert.Null(result);
        }
    }
}
