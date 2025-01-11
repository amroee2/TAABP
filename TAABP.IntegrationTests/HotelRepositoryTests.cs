using AutoFixture;
using Microsoft.EntityFrameworkCore;
using TAABP.Core;
using TAABP.Infrastructure;
using TAABP.Infrastructure.Repositories;

namespace TAABP.IntegrationTests
{
    public class HotelRepositoryTests
    {
        private readonly TAABPDbContext _context;
        private readonly HotelRepository _hotelRepository;
        private readonly IFixture _fixture;

        public HotelRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TAABPDbContext>()
             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
             .Options;
            _context = new TAABPDbContext(options);

            _hotelRepository = new HotelRepository(_context);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
        [Fact]
        public async Task CreateHotelAsync_ShouldAddHotelAsync()
        {
            // Arrange
            var hotel = _fixture.Create<Hotel>();

            // Act
            await _hotelRepository.CreateHotelAsync(hotel);

            // Assert
            var result = await _context.Hotels.FirstOrDefaultAsync(h=>h.HotelId == hotel.HotelId);
            Assert.Equal(hotel, result);
        }

        [Fact]
        public async Task GetHotelByIdAsync_ShouldReturnHotelByIdAsync()
        {
            // Arrange
            var hotel = _fixture.Create<Hotel>();
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();

            // Act
            var result = await _hotelRepository.GetHotelByIdAsync(hotel.HotelId);

            // Assert
            Assert.Equal(hotel.HotelId, result.HotelId);
        }

        [Fact]
        public async Task GetHotelsAsync_ShouldReturnAllCityHotelsAsync()
        {
            // Arrange
            var city = _fixture.Create<City>();
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();
            _fixture.Customize<Hotel>(c => c.With(h => h.CityId, city.CityId));

            var hotels = _fixture.CreateMany<Hotel>(3).ToList();
            await _context.Hotels.AddRangeAsync(hotels);
            await _context.SaveChangesAsync();

            // Act
            var result = await _hotelRepository.GetHotelsAsync(city.CityId);

            // Assert
            Assert.Equal(hotels.Count, result.Count);
            Assert.All(result, h => Assert.Equal(city.CityId, h.CityId));
        }

        [Fact]
        public async Task DeleteHotelAsync_ShouldDeleteHotelAsync()
        {
            // Arrange
            var hotel = _fixture.Create<Hotel>();
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();

            // Act
            await _hotelRepository.DeleteHotelAsync(hotel);

            // Assert
            var result = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == hotel.HotelId);
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateHotelAsync_ShouldUpdateHotelAsync()
        {
            // Arrange
            var hotel = _fixture.Create<Hotel>();
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();

            hotel.Name = "Updated Hotel Name";

            // Act
            await _hotelRepository.UpdateHotelAsync(hotel);

            // Assert
            var result = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == hotel.HotelId);
            Assert.Equal(hotel.Name, result.Name);
        }

        [Fact]
        public async Task CreateNewHotelImageAsync_ShouldAddHotelImageAsync()
        {
            // Arrange
            var hotelImage = _fixture.Create<HotelImage>();

            // Act
            await _hotelRepository.CreateNewHotelImageAsync(hotelImage);

            // Assert
            var result = await _context.HotelImages.FirstOrDefaultAsync(h => h.HotelImageId == hotelImage.HotelImageId);
            Assert.Equal(hotelImage, result);
        }

        [Fact]
        public async Task GetHotelImageByIdAsync_ShouldReturnHotelImageByIdAsync()
        {
            // Arrange
            var hotelImage = _fixture.Create<HotelImage>();
            await _context.HotelImages.AddAsync(hotelImage);
            await _context.SaveChangesAsync();

            // Act
            var result = await _hotelRepository.GetHotelImageByIdAsync(hotelImage.HotelId, hotelImage.HotelImageId);

            // Assert
            Assert.Equal(hotelImage.HotelImageId, result.HotelImageId);
        }

        [Fact]
        public async Task GetHotelImagesAsync_ShouldReturnAllHotelImagesAsync()
        {
            // Arrange
            var hotel = _fixture.Create<Hotel>();
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();
            _fixture.Customize<HotelImage>(c => c.With(h => h.HotelId, hotel.HotelId));

            var hotelImages = _fixture.CreateMany<HotelImage>(3).ToList();
            await _context.HotelImages.AddRangeAsync(hotelImages);
            await _context.SaveChangesAsync();

            // Act
            var result = await _hotelRepository.GetHotelImagesAsync(hotel.HotelId);

            // Assert
            Assert.Equal(hotelImages.Count, result.Count);
            Assert.All(result, img => Assert.Equal(hotel.HotelId, img.HotelId));
        }


        [Fact]
        public async Task DeleteHotelImageAsync_ShouldDeleteHotelImageAsync()
        {
            // Arrange
            var hotelImage = _fixture.Create<HotelImage>();
            await _context.HotelImages.AddAsync(hotelImage);
            await _context.SaveChangesAsync();

            // Act
            await _hotelRepository.DeleteHotelImageAsync(hotelImage);

            // Assert
            var result = await _context.HotelImages.FirstOrDefaultAsync(h => h.HotelImageId == hotelImage.HotelImageId);
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateHotelImageAsync_ShouldUpdateHotelImageAsync()
        {
            // Arrange
            var hotelImage = _fixture.Create<HotelImage>();
            await _context.HotelImages.AddAsync(hotelImage);
            await _context.SaveChangesAsync();

            hotelImage.ImageUrl = "Updated Image Url";

            // Act
            await _hotelRepository.UpdateHotelImageAsync(hotelImage);

            // Assert
            var result = await _context.HotelImages.FirstOrDefaultAsync(h => h.HotelImageId == hotelImage.HotelImageId);
            Assert.Equal(hotelImage.ImageUrl, result.ImageUrl);
        }

        [Fact]
        public async Task IncrementNumberOfVisits_ShouldIncrementNumberOfVisitsAsync()
        {
            // Arrange
            var hotel = _fixture.Create<Hotel>();
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();

            // Act
            await _hotelRepository.IncrementNumberOfVisitsAsync(hotel.HotelId);

            // Assert
            var result = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == hotel.HotelId);
            Assert.Equal(hotel.NumberOfVisits, result.NumberOfVisits);
        }

        [Fact]
        public async Task DecrementNumberOfVisits_ShouldDecrementNumberOfVisitsAsync()
        {
            // Arrange
            var hotel = _fixture.Build<Hotel>()
                .With(h => h.NumberOfVisits, 10)
                .Create();
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();

            // Act
            await _hotelRepository.DecrementNumberOfVisitsAsync(hotel.HotelId);

            // Assert
            var result = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == hotel.HotelId);
            Assert.Equal(9, result.NumberOfVisits);
        }
    }
}