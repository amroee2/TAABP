using AutoFixture;
using Moq;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.AmenityMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.Services;
using TAABP.Core;

namespace TAABP.UnitTests
{
    public class AmenityServiceTests
    {
        private readonly AmenityService _amenityService;
        private readonly Mock<IAmenityRepository> _amenityRepositoryMock;
        private readonly Mock<IAmenityMapper> _amenityMapperMock;
        private readonly Mock<IHotelRepository> _hotelRepositoryMock;
        private readonly IFixture _fixture;

        public AmenityServiceTests()
        {
            _amenityRepositoryMock = new Mock<IAmenityRepository>();
            _amenityMapperMock = new Mock<IAmenityMapper>();
            _hotelRepositoryMock = new Mock<IHotelRepository>();
            _amenityService = new AmenityService(_amenityMapperMock.Object, _amenityRepositoryMock.Object, _hotelRepositoryMock.Object);
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetHotelAmenitiesAsync_ShouldReturnListOfAmenities()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var hotel = _fixture.Create<Hotel>();
            var amenities = _fixture.Create<List<Amenity>>();
            var amenityDtos = amenities.Select(amenity => _fixture.Create<AmenityDto>()).ToList();
            _hotelRepositoryMock.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync(hotel);
            _amenityRepositoryMock.Setup(x => x.GetHotelAmenitiesAsync(hotelId)).ReturnsAsync(amenities);
            _amenityMapperMock.Setup(x => x.AmenityToAmenityDto(It.IsAny<Amenity>())).Returns(amenityDtos.First());

            // Act
            var result = await _amenityService.GetHotelAmenitiesAsync(hotelId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(amenities.Count, result.Count);
        }

        [Fact]
        public async Task GetHotelAmenitiesAsync_ShouldThrowEntityNotFoundExceptionAsync()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var hotel = _fixture.Create<Hotel>();
            var amenities = _fixture.Create<List<Amenity>>();
            var amenityDtos = amenities.Select(amenity => _fixture.Create<AmenityDto>()).ToList();
            _hotelRepositoryMock.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync((Hotel)null);
            _amenityRepositoryMock.Setup(x => x.GetHotelAmenitiesAsync(hotelId)).ReturnsAsync(amenities);
            _amenityMapperMock.Setup(x => x.AmenityToAmenityDto(It.IsAny<Amenity>())).Returns(amenityDtos.First());

            // Act
            async Task Act() => await _amenityService.GetHotelAmenitiesAsync(hotelId);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);

        }
        [Fact]
        public async Task GetAmenityByIdAsync_ShouldReturnAmenity()
        {
            // Arrange
            var amenityId = _fixture.Create<int>();
            var hotel = _fixture.Create<Hotel>();
            _fixture.Customize<Amenity>(c => c.With(p => p.HotelId, hotel.HotelId).With(p=>p.AmenityId, amenityId));
            var amenity = _fixture.Create<Amenity>();
            var amenityDto = _fixture.Create<AmenityDto>();
            _hotelRepositoryMock.Setup(x => x.GetHotelByIdAsync(hotel.HotelId)).ReturnsAsync(hotel);
            _amenityRepositoryMock.Setup(x => x.GetAmenityByIdAsync(amenityId)).ReturnsAsync(amenity);
            _amenityMapperMock.Setup(x => x.AmenityToAmenityDto(It.IsAny<Amenity>())).Returns(amenityDto);

            // Act
            var result = await _amenityService.GetAmenityByIdAsync(hotel.HotelId, amenityId);

            // Assert
            Assert.Equal(amenityDto, result);
        }

        [Fact]
        public async Task GetAmenityByIdAsync_ShouldThrowEntityNotFoundExceptionAsync()
        {
            // Arrange
            var amenityId = _fixture.Create<int>();
            var hotel = _fixture.Create<Hotel>();
            _fixture.Customize<Amenity>(c => c.With(p => p.HotelId, hotel.HotelId).With(p => p.AmenityId, amenityId));
            var amenity = _fixture.Create<Amenity>();
            var amenityDto = _fixture.Create<AmenityDto>();
            _hotelRepositoryMock.Setup(x => x.GetHotelByIdAsync(hotel.HotelId)).ReturnsAsync(hotel);
            _amenityRepositoryMock.Setup(x => x.GetAmenityByIdAsync(amenityId)).ReturnsAsync((Amenity)null);
            _amenityMapperMock.Setup(x => x.AmenityToAmenityDto(It.IsAny<Amenity>())).Returns(amenityDto);

            // Act
            async Task Act() => await _amenityService.GetAmenityByIdAsync(hotel.HotelId, amenityId);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        [Fact]
        public async Task CreateAmenityAsync_ShouldCreateAmenity()
        {
            // Arrange
            var amenityDto = _fixture.Create<AmenityDto>();
            var hotel = _fixture.Create<Hotel>();
            var amenityId = _fixture.Create<int>(); 
            var amenity = new Amenity { AmenityId = amenityId };

            _hotelRepositoryMock
                .Setup(x => x.GetHotelByIdAsync(amenityDto.HotelId))
                .ReturnsAsync(hotel);

            _amenityMapperMock
                .Setup(x => x.AmenityDtoToAmenity(amenityDto, It.IsAny<Amenity>()))
                .Callback((AmenityDto dto, Amenity entity) =>
                {
                    entity.AmenityId = amenityId;
                });

            _amenityRepositoryMock
                .Setup(x => x.CreateAmenityAsync(It.IsAny<Amenity>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _amenityService.CreateAmenityAsync(amenityDto);

            // Assert
            Assert.Equal(amenityId, result);
        }

        [Fact]
        public async Task CreateAmenityAsync_ShouldThrowEntityNotFoundExceptionAsync()
        {
            // Arrange
            var amenityDto = _fixture.Create<AmenityDto>();
            var hotel = _fixture.Create<Hotel>();
            var amenityId = _fixture.Create<int>();
            var amenity = new Amenity { AmenityId = amenityId };

            _hotelRepositoryMock
                .Setup(x => x.GetHotelByIdAsync(amenityDto.HotelId))
                .ReturnsAsync((Hotel)null);

            _amenityMapperMock
                .Setup(x => x.AmenityDtoToAmenity(amenityDto, It.IsAny<Amenity>()))
                .Callback((AmenityDto dto, Amenity entity) =>
                {
                    entity.AmenityId = amenityId;
                });

            _amenityRepositoryMock
                .Setup(x => x.CreateAmenityAsync(It.IsAny<Amenity>()))
                .Returns(Task.CompletedTask);

            // Act
            async Task Act() => await _amenityService.CreateAmenityAsync(amenityDto);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        [Fact]
        public async Task UpdateAmenityAsync_ShouldUpdateAmenityAsync()
        {
            // Arrange
            var amenityDto = _fixture.Create<AmenityDto>();
            var hotel = _fixture.Build<Hotel>().With(h => h.HotelId, amenityDto.HotelId).Create();
            var amenity = _fixture.Build<Amenity>().With(a => a.HotelId, amenityDto.HotelId).Create();

            _hotelRepositoryMock
                .Setup(x => x.GetHotelByIdAsync(amenityDto.HotelId))
                .ReturnsAsync(hotel);

            _amenityRepositoryMock
                .Setup(x => x.GetAmenityByIdAsync(amenityDto.AmenityId))
                .ReturnsAsync(amenity);

            _amenityRepositoryMock
                .Setup(x => x.UpdateAmenityAsync(It.IsAny<Amenity>()));

            _amenityMapperMock
                .Setup(x => x.AmenityDtoToAmenity(amenityDto, amenity))
                .Verifiable();

            // Act
            await _amenityService.UpdateAmenityAsync(amenityDto);

            // Assert
            _amenityMapperMock.Verify(x => x.AmenityDtoToAmenity(amenityDto, amenity), Times.Once);
            _amenityRepositoryMock.Verify(x => x.UpdateAmenityAsync(amenity), Times.Once);
        }

        [Fact]
        public async Task UpdateAmenityAsync_ShouldThrowEntityNotFoundExceptionAsync()
        {
            // Arrange
            var amenityDto = _fixture.Create<AmenityDto>();
            var hotel = _fixture.Build<Hotel>().With(h => h.HotelId, amenityDto.HotelId).Create();
            var amenity = _fixture.Build<Amenity>().With(a => a.HotelId, amenityDto.HotelId).Create();

            _hotelRepositoryMock
                .Setup(x => x.GetHotelByIdAsync(amenityDto.HotelId))
                .ReturnsAsync(hotel);

            _amenityRepositoryMock
                .Setup(x => x.GetAmenityByIdAsync(amenityDto.AmenityId))
                .ReturnsAsync((Amenity)null);

            _amenityRepositoryMock
                .Setup(x => x.UpdateAmenityAsync(It.IsAny<Amenity>()));

            _amenityMapperMock
                .Setup(x => x.AmenityDtoToAmenity(amenityDto, amenity))
                .Verifiable();

            // Act
            async Task Act() => await _amenityService.UpdateAmenityAsync(amenityDto);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        [Fact]
        public async Task DeleteAmenityAsync_ShouldDeleteAmenityAsync()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var amenityId = _fixture.Create<int>();
            var hotel = _fixture.Build<Hotel>().With(h => h.HotelId, hotelId).Create();
            var amenity = _fixture.Build<Amenity>().With(a => a.HotelId, hotelId).With(a => a.AmenityId, amenityId).Create();

            _hotelRepositoryMock
                .Setup(x => x.GetHotelByIdAsync(hotelId))
                .ReturnsAsync(hotel);

            _amenityRepositoryMock
                .Setup(x => x.GetAmenityByIdAsync(amenityId))
                .ReturnsAsync(amenity);

            _amenityRepositoryMock
                .Setup(x => x.DeleteAmenityAsync(amenity))
                .Returns(Task.CompletedTask);

            // Act
            await _amenityService.DeleteAmenityAsync(hotelId, amenityId);

            // Assert
            _amenityRepositoryMock.Verify(x => x.DeleteAmenityAsync(amenity), Times.Once);
        }


        [Fact]
        public async Task DeleteAmenityAsync_ShouldThrowEntityNotFoundExceptionAsync()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var amenityId = _fixture.Create<int>();
            var hotel = _fixture.Create<Hotel>();
            var amenity = _fixture.Build<Amenity>().With(a => a.HotelId, hotelId).Create();

            _hotelRepositoryMock
                .Setup(x => x.GetHotelByIdAsync(hotelId))
                .ReturnsAsync(hotel);

            _amenityRepositoryMock
                .Setup(x => x.GetAmenityByIdAsync(amenityId))
                .ReturnsAsync((Amenity)null);

            _amenityRepositoryMock
                .Setup(x => x.DeleteAmenityAsync(amenity));

            // Act
            async Task Act() => await _amenityService.DeleteAmenityAsync(hotelId, amenityId);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }
    }
}
