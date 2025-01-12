using AutoFixture;
using Microsoft.EntityFrameworkCore;
using TAABP.Core;
using TAABP.Infrastructure;
using TAABP.Infrastructure.Repositories;

namespace TAABP.IntegrationTests
{
    public class AmenityRepositoryTests
    {
        private readonly TAABPDbContext _context;
        private readonly AmenityRepository _amenityRepository;
        private readonly IFixture _fixture;

        public AmenityRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TAABPDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TAABPDbContext(options);
            _amenityRepository = new AmenityRepository(_context);
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task CreateAmenityAsync_ShouldAddAmenityAsync()
        {
            // Arrange
            var amenity = _fixture.Create<Amenity>();
            // Act
            await _amenityRepository.CreateAmenityAsync(amenity);

            // Assert
            var result = await _context.Amenities.FirstOrDefaultAsync(a => a.AmenityId == amenity.AmenityId);
            Assert.Equal(amenity, result);
        }

        [Fact]
        public async Task GetAmenityByIdAsync_ShouldReturnAmenityByIdAsync()
        {
            // Arrange
            var amenity = _fixture.Create<Amenity>();
            await _context.Amenities.AddAsync(amenity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _amenityRepository.GetAmenityByIdAsync(amenity.AmenityId);

            // Assert
            Assert.Equal(amenity.AmenityId, result.AmenityId);
        }

        [Fact]
        public async Task GetAmenitiesAsync_ShouldReturnAllAmenitiesAsync()
        {
            // Arrange
            var hotel = _fixture.Create<Hotel>();
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();
            _fixture.Customize<Amenity>(a => a.With(h => h.HotelId, hotel.HotelId));
            var amenities = _fixture.CreateMany<Amenity>().ToList();
            await _context.Amenities.AddRangeAsync(amenities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _amenityRepository.GetHotelAmenitiesAsync(hotel.HotelId);

            // Assert
            Assert.Equal(amenities.Count, result.Count());
            Assert.All(result, amenity => Assert.Equal(hotel.HotelId, amenity.HotelId));
        }

        [Fact]
        public async Task UpdateAmenityAsync_ShouldUpdateAmenityAsync()
        {
            // Arrange
            var amenity = _fixture.Create<Amenity>();
            await _context.Amenities.AddAsync(amenity);
            await _context.SaveChangesAsync();
            amenity.Name = "Updated Name";

            // Act
            await _amenityRepository.UpdateAmenityAsync(amenity);

            // Assert
            var result = await _context.Amenities.FirstOrDefaultAsync(a => a.AmenityId == amenity.AmenityId);
            Assert.Equal(amenity.Name, result.Name);
        }

        [Fact]
        public async Task DeleteAmenityAsync_ShouldDeleteAmenityAsync()
        {
            // Arrange
            var amenity = _fixture.Create<Amenity>();
            await _context.Amenities.AddAsync(amenity);
            await _context.SaveChangesAsync();

            // Act
            await _amenityRepository.DeleteAmenityAsync(amenity);

            // Assert
            var result = await _context.Amenities.FirstOrDefaultAsync(a => a.AmenityId == amenity.AmenityId);
            Assert.Null(result);
        }
    }
}
