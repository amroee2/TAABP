using AutoFixture;
using Microsoft.EntityFrameworkCore;
using TAABP.Core;
using TAABP.Infrastructure;
using TAABP.Infrastructure.Repositories;

namespace TAABP.IntegrationTests
{
    public class FeaturedDealRepositoryTests
    {
        private readonly TAABPDbContext _context;
        private readonly FeaturedDealRepository _featuredDealRepository;
        private readonly IFixture _fixture;

        public FeaturedDealRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TAABPDbContext>()
             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
             .Options;
            _context = new TAABPDbContext(options);

            _featuredDealRepository = new FeaturedDealRepository(_context);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task CreateFeaturedDealAsync_ShouldAddFeaturedDealAsync()
        {
            // Arrange
            var featuredDeal = _fixture.Create<FeaturedDeal>();

            // Act
            await _featuredDealRepository.CreateFeaturedDealAsync(featuredDeal);

            // Assert
            var result = await _context.FeaturedDeals.FirstOrDefaultAsync(fd => fd.FeaturedDealId == featuredDeal.FeaturedDealId);
            Assert.Equal(featuredDeal, result);
        }

        [Fact]
        public async Task GetFeaturedDealByIdAsync_ShouldReturnFeaturedDealByIdAsync()
        {
            // Arrange
            var featuredDeal = _fixture.Create<FeaturedDeal>();
            await _context.FeaturedDeals.AddAsync(featuredDeal);
            await _context.SaveChangesAsync();

            // Act
            var result = await _featuredDealRepository.GetFeaturedDealAsync(featuredDeal.FeaturedDealId);

            // Assert
            Assert.Equal(featuredDeal.FeaturedDealId, result.FeaturedDealId);
        }

        [Fact]
        public async Task GetFeaturedDealsAsync_ShouldReturnAllFeaturedDealsAsync()
        {
            // Arrange
            var featuredDeals = _fixture.CreateMany<FeaturedDeal>().ToList();
            await _context.FeaturedDeals.AddRangeAsync(featuredDeals);
            await _context.SaveChangesAsync();

            // Act
            var result = await _featuredDealRepository.GetFeaturedDealsAsync();

            // Assert
            Assert.Equal(featuredDeals.Count, result.Count);
        }

        [Fact]
        public async Task GetFeaturedDealsByHotelIdAsync_ShouldReturnFeaturedDealsByHotelIdAsync()
        {
            // Arrange
            var featuredDeals = _fixture.CreateMany<FeaturedDeal>().ToList();
            await _context.FeaturedDeals.AddRangeAsync(featuredDeals);
            await _context.SaveChangesAsync();

            // Act
            var result = await _featuredDealRepository.GetFeaturedDealsAsync();

            // Assert
            Assert.Equal(featuredDeals.Count, result.Count);
        }

        [Fact]
        public async Task DeleteFeaturedDealAsync_ShouldDeleteFeaturedDealAsync()
        {
            // Arrange
            var featuredDeal = _fixture.Create<FeaturedDeal>();
            await _context.FeaturedDeals.AddAsync(featuredDeal);
            await _context.SaveChangesAsync();

            // Act
            await _featuredDealRepository.DeleteFeaturedDealAsync(featuredDeal);

            // Assert
            var result = await _context.FeaturedDeals.FirstOrDefaultAsync(fd => fd.FeaturedDealId == featuredDeal.FeaturedDealId);
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateFeaturedDealAsync_ShouldUpdateFeaturedDealAsync()
        {
            // Arrange
            var featuredDeal = _fixture.Create<FeaturedDeal>();
            await _context.FeaturedDeals.AddAsync(featuredDeal);
            await _context.SaveChangesAsync();

            // Act
            featuredDeal.Title = "Updated Title";
            await _featuredDealRepository.UpdateFeaturedDealAsync(featuredDeal);

            // Assert
            var result = await _context.FeaturedDeals.FirstOrDefaultAsync(fd => fd.FeaturedDealId == featuredDeal.FeaturedDealId);
            Assert.Equal("Updated Title", result.Title);
        }
    }
}
