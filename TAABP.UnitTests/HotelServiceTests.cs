using AutoFixture;
using Moq;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.HotelMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.Services;
using TAABP.Core;

namespace TAABP.UnitTests
{
    public class HotelServiceTests
    {
        private readonly Mock<ICityRepository> _mockCityRepository;
        private readonly Mock<IHotelRepository> _mockHotelRepository;
        private readonly Mock<IHotelMapper> _mockHotelMapper;
        private readonly Mock<IUserService> _mockUserService;
        private readonly HotelService _hotelService;
        private readonly IFixture _fixture;

        public HotelServiceTests()
        {
            _mockCityRepository = new Mock<ICityRepository>();
            _mockHotelRepository = new Mock<IHotelRepository>();
            _mockHotelMapper = new Mock<IHotelMapper>();
            _mockUserService = new Mock<IUserService>();

            _hotelService = new HotelService(
                _mockCityRepository.Object,
                _mockHotelRepository.Object,
                _mockHotelMapper.Object,
                _mockUserService.Object
            );

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task CreateHotelAsync_ShouldThrowEntityNotFoundException_WhenCityDoesNotExist()
        {
            // Arrange
            var cityId = _fixture.Create<int>();
            var hotelDto = _fixture.Create<HotelDto>();
            _mockCityRepository.Setup(x => x.GetCityByIdAsync(cityId)).ReturnsAsync((City)null);

            // Act
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _hotelService.CreateHotelAsync(cityId, hotelDto));

            // Assert
            Assert.Equal($"City with id {cityId} not found", exception.Message);
            _mockCityRepository.Verify(x => x.GetCityByIdAsync(cityId), Times.Once);
            _mockHotelRepository.Verify(x => x.CreateHotelAsync(It.IsAny<Hotel>()), Times.Never);
        }

        [Fact]
        public async Task CreateHotelAsync_ShouldCreateHotel_WhenCityExists()
        {
            // Arrange
            var cityId = _fixture.Create<int>();
            var hotelDto = _fixture.Create<HotelDto>();
            var city = _fixture.Build<City>().With(c => c.CityId, cityId).Create();
            var hotel = _fixture.Create<Hotel>();

            _mockCityRepository.Setup(x => x.GetCityByIdAsync(cityId)).ReturnsAsync(city);
            _mockUserService.Setup(x => x.GetCurrentUsernameAsync()).ReturnsAsync(_fixture.Create<string>());
            _mockHotelRepository.Setup(x => x.CreateHotelAsync(It.IsAny<Hotel>()))
                .Callback<Hotel>(h => h.HotelId = hotel.HotelId)
                .Returns(Task.CompletedTask);
            _mockCityRepository.Setup(x => x.IncrementNumberOfHotelsAsync(cityId)).Returns(Task.CompletedTask);
            _mockHotelMapper.Setup(x => x.HotelDtoToHotel(hotelDto, It.IsAny<Hotel>()))
                .Callback<HotelDto, Hotel>((dto, h) => h.Name = hotel.Name);

            // Act
            var result = await _hotelService.CreateHotelAsync(cityId, hotelDto);

            // Assert
            Assert.Equal(hotel.HotelId, result);
            _mockCityRepository.Verify(x => x.GetCityByIdAsync(cityId), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelDtoToHotel(hotelDto, It.IsAny<Hotel>()), Times.Once);
            _mockHotelRepository.Verify(x => x.CreateHotelAsync(It.IsAny<Hotel>()), Times.Once);
            _mockCityRepository.Verify(x => x.IncrementNumberOfHotelsAsync(cityId), Times.Once);
            _mockUserService.Verify(x => x.GetCurrentUsernameAsync(), Times.Once);
        }

        [Fact]
        public async Task GetHotelByIdAsync_ShouldThrowEntityNotFoundException_WhenHotelDoesNotExist()
        {
            // Arrange
            var cityId = _fixture.Create<int>();
            var hotelId = _fixture.Create<int>();
            _mockHotelRepository.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync((Hotel)null);
            _mockCityRepository.Setup(x => x.GetCityByIdAsync(cityId)).ReturnsAsync(new City());
            // Act
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _hotelService.GetHotelByIdAsync(cityId, hotelId));

            // Assert
            Assert.Equal($"Hotel with id {hotelId} not found", exception.Message);
            _mockHotelRepository.Verify(x => x.GetHotelByIdAsync(hotelId), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelToHotelDto(It.IsAny<Hotel>()), Times.Never);
        }

        [Fact]
        public async Task GetHotelByIdAsync_ShouldReturnHotelDto_WhenHotelExists()
        {
            // Arrange

            var cityId = _fixture.Create<int>();
            var hotelId = _fixture.Create<int>();
            _fixture.Customize<Hotel>(c => c.With(h => h.HotelId, hotelId).With(h=>h.CityId,cityId));
            _fixture.Customize<City>(City => City.With(c => c.CityId, cityId));
            var hotel = _fixture.Create<Hotel>();
            var city = _fixture.Create<City>();
            var hotelDto = _fixture.Build<HotelDto>().With(d => d.HotelId, hotel.HotelId).Create();

            _mockHotelRepository.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync(hotel);
            _mockCityRepository.Setup(x => x.GetCityByIdAsync(cityId)).ReturnsAsync(city);
            _mockHotelMapper.Setup(x => x.HotelToHotelDto(hotel)).Returns(hotelDto);

            // Act
            var result = await _hotelService.GetHotelByIdAsync(cityId, hotelId);

            // Assert
            Assert.Equal(hotelDto, result);
            _mockHotelRepository.Verify(x => x.GetHotelByIdAsync(hotelId), Times.Once);
            _mockCityRepository.Verify(x => x.GetCityByIdAsync(cityId), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelToHotelDto(hotel), Times.Once);
        }

        [Fact]
        public async Task GetHotelsAsync_ShouldReturnListOfHotelDtos()
        {
            // Arrange
            var cityId = _fixture.Create<int>();
            var hotels = _fixture.CreateMany<Hotel>().ToList();
            var hotelDtos = hotels.Select(hotel => _fixture.Build<HotelDto>().With(d => d.HotelId, hotel.HotelId).Create()).ToList();

            _mockHotelRepository.Setup(x => x.GetHotelsAsync(cityId)).ReturnsAsync(hotels);
            _mockHotelMapper.SetupSequence(x => x.HotelToHotelDto(It.IsAny<Hotel>()))
                .Returns(hotelDtos[0])
                .Returns(hotelDtos[1])
                .Returns(hotelDtos[2]);

            // Act
            var result = await _hotelService.GetHotelsAsync(cityId);

            // Assert
            Assert.Equal(hotelDtos, result);
            _mockHotelRepository.Verify(x => x.GetHotelsAsync(cityId), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelToHotelDto(hotels[0]), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelToHotelDto(hotels[1]), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelToHotelDto(hotels[2]), Times.Once);
        }

        [Fact]
        public async Task DeleteHotelAsync_ShouldThrowEntityNotFoundException_WhenHotelDoesNotExist()
        {
            // Arrange
            var cityId = _fixture.Create<int>();
            var hotelId = _fixture.Create<int>();
            _fixture.Customize<Hotel>(c => c.With(h => h.HotelId, hotelId).With(h => h.CityId, cityId));
            _fixture.Customize<City>(City => City.With(c => c.CityId, cityId));
            _mockHotelRepository.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync((Hotel)null);
            _mockCityRepository.Setup(x => x.GetCityByIdAsync(cityId)).ReturnsAsync(new City());
            // Act
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _hotelService.DeleteHotelAsync(cityId, hotelId));

            // Assert
            Assert.Equal($"Hotel with id {hotelId} not found", exception.Message);
            _mockHotelRepository.Verify(x => x.GetHotelByIdAsync(hotelId), Times.Once);
            _mockHotelRepository.Verify(x => x.DeleteHotelAsync(It.IsAny<Hotel>()), Times.Never);
        }

        [Fact]
        public async Task DeleteHotelAsync_ShouldDeleteHotel_WhenHotelExists()
        {
            // Arrange
            var cityId = _fixture.Create<int>();
            var hotelId = _fixture.Create<int>();
            _fixture.Customize<Hotel>(c => c.With(h => h.HotelId, hotelId).With(h => h.CityId, cityId));
            _fixture.Customize<City>(City => City.With(c => c.CityId, cityId));
            var hotel = _fixture.Create<Hotel>();
            var city = _fixture.Create<City>();

            _mockHotelRepository.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync(hotel);
            _mockCityRepository.Setup(x => x.GetCityByIdAsync(cityId)).ReturnsAsync(city);
            _mockHotelRepository.Setup(x => x.DeleteHotelAsync(hotel)).Returns(Task.CompletedTask);
            _mockCityRepository.Setup(x => x.DecrementNumberOfHotelsAsync(cityId)).Returns(Task.CompletedTask);

            // Act
            await _hotelService.DeleteHotelAsync(cityId, hotelId);

            // Assert
            _mockHotelRepository.Verify(x => x.GetHotelByIdAsync(hotelId), Times.Once);
            _mockCityRepository.Verify(x => x.GetCityByIdAsync(cityId), Times.Once);
            _mockHotelRepository.Verify(x => x.DeleteHotelAsync(hotel), Times.Once);
            _mockCityRepository.Verify(x => x.DecrementNumberOfHotelsAsync(cityId), Times.Once);
        }

        [Fact]
        public async Task UpdateHotelAsync_ShouldThrowEntityNotFoundException_WhenHotelDoesNotExist()
        {
            // Arrange
            var cityId = _fixture.Create<int>();
            var hotelDto = _fixture.Create<HotelDto>();
            _mockHotelRepository.Setup(x => x.GetHotelByIdAsync(hotelDto.HotelId)).ReturnsAsync((Hotel)null);
            _mockCityRepository.Setup(x => x.GetCityByIdAsync(cityId)).ReturnsAsync(new City());
            // Act
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _hotelService.UpdateHotelAsync(cityId, hotelDto));

            // Assert
            Assert.Equal($"Hotel with id {hotelDto.HotelId} not found", exception.Message);
            _mockHotelRepository.Verify(x => x.GetHotelByIdAsync(hotelDto.HotelId), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelDtoToHotel(It.IsAny<HotelDto>(), It.IsAny<Hotel>()), Times.Never);
            _mockHotelRepository.Verify(x => x.UpdateHotelAsync(It.IsAny<Hotel>()), Times.Never);
        }

        [Fact]
        public async Task UpdateHotelAsync_ShouldUpdateHotel_WhenHotelExists()
        {
            // Arrange
            var cityId = _fixture.Create<int>();
            var hotelId = _fixture.Create<int>();
            _fixture.Customize<Hotel>(c => c.With(h => h.HotelId, hotelId).With(h => h.CityId, cityId));
            _fixture.Customize<City>(City => City.With(c => c.CityId, cityId));
            var hotelDto = _fixture.Create<HotelDto>();
            var hotel = _fixture.Create<Hotel>();
            var city = _fixture.Create<City>();

            _mockHotelRepository.Setup(x => x.GetHotelByIdAsync(hotelDto.HotelId)).ReturnsAsync(hotel);
            _mockCityRepository.Setup(x => x.GetCityByIdAsync(cityId)).ReturnsAsync(city);
            _mockHotelMapper.Setup(x => x.HotelDtoToHotel(hotelDto, hotel));
            _mockHotelRepository.Setup(x => x.UpdateHotelAsync(hotel)).Returns(Task.CompletedTask);

            // Act
            await _hotelService.UpdateHotelAsync(cityId, hotelDto);

            // Assert
            _mockHotelRepository.Verify(x => x.GetHotelByIdAsync(hotelDto.HotelId), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelDtoToHotel(hotelDto, hotel), Times.Once);
            _mockHotelRepository.Verify(x => x.UpdateHotelAsync(hotel), Times.Once);
        }

        [Fact]
        public async Task CreateNewHotelImageAsync_WithValidHotelIdAndDto_CreatesHotelImage()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var hotelImageId = _fixture.Create<int>();
            var hotel = _fixture.Create<Hotel>();
            var hotelImageDto = _fixture.Create<HotelImageDto>();
            var hotelImage = _fixture.Build<HotelImage>()
                .With(h => h.HotelImageId, hotelImageId)
                .Create();

            _mockHotelRepository.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync(hotel);
            _mockHotelRepository.Setup(x => x.CreateNewHotelImageAsync(It.IsAny<HotelImage>()))
                .Callback<HotelImage>(h => h.HotelImageId = hotelImageId)
                .Returns(Task.CompletedTask);
            _mockHotelMapper.Setup(x => x.HotelImageDtoToHotelImage(hotelImageDto, It.IsAny<HotelImage>()))
                .Callback<HotelImageDto, HotelImage>((dto, image) =>
                {
                    image.ImageUrl = dto.ImageUrl;
                });

            // Act
            var result = await _hotelService.CreateNewHotelImageAsync(hotelId, hotelImageDto);

            // Assert
            Assert.Equal(hotelImageId, result);
            _mockHotelRepository.Verify(x => x.GetHotelByIdAsync(hotelId), Times.Once);
            _mockHotelRepository.Verify(x => x.CreateNewHotelImageAsync(It.IsAny<HotelImage>()), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelImageDtoToHotelImage(hotelImageDto, It.IsAny<HotelImage>()), Times.Once);
        }

        [Fact]
        public async Task CreateNewHotelImageAsync_WithInvalidHotelId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var hotelImageDto = _fixture.Create<HotelImageDto>();

            _mockHotelRepository.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync((Hotel)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _hotelService.CreateNewHotelImageAsync(hotelId, hotelImageDto));
            Assert.Equal($"Hotel with id {hotelId} not found", exception.Message);

            _mockHotelRepository.Verify(x => x.GetHotelByIdAsync(hotelId), Times.Once);
            _mockHotelRepository.Verify(x => x.CreateNewHotelImageAsync(It.IsAny<HotelImage>()), Times.Never);
            _mockHotelMapper.Verify(x => x.HotelImageDtoToHotelImage(It.IsAny<HotelImageDto>(), It.IsAny<HotelImage>()), Times.Never);
        }

        [Fact]
        public async Task GetHotelImageByIdAsync_WithValidHotelIdAndImageId_ReturnsHotelImageDto()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var imageId = _fixture.Create<int>();
            var hotelImage = _fixture.Create<HotelImage>();
            var hotelImageDto = _fixture.Build<HotelImageDto>().With(d => d.HotelImageId, imageId).Create();

            _mockHotelRepository.Setup(x => x.GetHotelImageByIdAsync(hotelId, imageId)).ReturnsAsync(hotelImage);
            _mockHotelMapper.Setup(x => x.HotelImageToHotelImageDto(hotelImage)).Returns(hotelImageDto);

            // Act
            var result = await _hotelService.GetHotelImageByIdAsync(hotelId, imageId);

            // Assert
            Assert.Equal(hotelImageDto, result);
            _mockHotelRepository.Verify(x => x.GetHotelImageByIdAsync(hotelId, imageId), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelImageToHotelImageDto(hotelImage), Times.Once);
        }

        [Fact]
        public async Task GetHotelImageByIdAsync_WithInvalidHotelIdOrImageId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var imageId = _fixture.Create<int>();

            _mockHotelRepository.Setup(x => x.GetHotelImageByIdAsync(hotelId, imageId)).ReturnsAsync((HotelImage)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _hotelService.GetHotelImageByIdAsync(hotelId, imageId));
            Assert.Equal($"Hotel Image with id {imageId} for hotel with id {hotelId} not found", exception.Message);

            _mockHotelRepository.Verify(x => x.GetHotelImageByIdAsync(hotelId, imageId), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelImageToHotelImageDto(It.IsAny<HotelImage>()), Times.Never);
        }

        [Fact]
        public async Task GetHotelImagesAsync_WithValidHotelId_ReturnsListOfHotelImageDtos()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var hotelImages = _fixture.CreateMany<HotelImage>().ToList();
            var hotelImageDtos = hotelImages.Select(image => _fixture.Build<HotelImageDto>().With(d => d.HotelImageId, image.HotelImageId).Create()).ToList();

            _mockHotelRepository.Setup(x => x.GetHotelImagesAsync(hotelId)).ReturnsAsync(hotelImages);
            _mockHotelMapper.SetupSequence(x => x.HotelImageToHotelImageDto(It.IsAny<HotelImage>()))
                .Returns(hotelImageDtos[0])
                .Returns(hotelImageDtos[1])
                .Returns(hotelImageDtos[2]);

            // Act
            var result = await _hotelService.GetHotelImagesAsync(hotelId);

            // Assert
            Assert.Equal(hotelImageDtos, result);
            _mockHotelRepository.Verify(x => x.GetHotelImagesAsync(hotelId), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelImageToHotelImageDto(hotelImages[0]), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelImageToHotelImageDto(hotelImages[1]), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelImageToHotelImageDto(hotelImages[2]), Times.Once);
        }

        [Fact]
        public async Task DeleteHotelImageAsync_WithValidHotelIdAndImageId_DeletesHotelImage()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var imageId = _fixture.Create<int>();
            var hotelImage = _fixture.Create<HotelImage>();

            _mockHotelRepository.Setup(x => x.GetHotelImageByIdAsync(hotelId, imageId)).ReturnsAsync(hotelImage);
            _mockHotelRepository.Setup(x => x.DeleteHotelImageAsync(hotelImage)).Returns(Task.CompletedTask);

            // Act
            await _hotelService.DeleteHotelImageAsync(hotelId, imageId);

            // Assert
            _mockHotelRepository.Verify(x => x.GetHotelImageByIdAsync(hotelId, imageId), Times.Once);
            _mockHotelRepository.Verify(x => x.DeleteHotelImageAsync(hotelImage), Times.Once);
        }

        [Fact]
        public async Task DeleteHotelImageAsync_WithInvalidHotelIdOrImageId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var imageId = _fixture.Create<int>();

            _mockHotelRepository.Setup(x => x.GetHotelImageByIdAsync(hotelId, imageId)).ReturnsAsync((HotelImage)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _hotelService.DeleteHotelImageAsync(hotelId, imageId));
            Assert.Equal($"Hotel Image with id {imageId} for hotel with id {hotelId} not found", exception.Message);

            _mockHotelRepository.Verify(x => x.GetHotelImageByIdAsync(hotelId, imageId), Times.Once);
            _mockHotelRepository.Verify(x => x.DeleteHotelImageAsync(It.IsAny<HotelImage>()), Times.Never);
        }

        [Fact]
        public async Task UpdateHotelImageAsync_WithValidHotelIdAndImageId_UpdatesHotelImage()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var imageId = _fixture.Create<int>();
            var hotelImage = _fixture.Create<HotelImage>();
            var hotelImageDto = _fixture.Create<HotelImageDto>();

            _mockHotelRepository.Setup(x => x.GetHotelImageByIdAsync(hotelId, imageId)).ReturnsAsync(hotelImage);
            _mockHotelMapper.Setup(x => x.HotelImageDtoToHotelImage(hotelImageDto, hotelImage));
            _mockHotelRepository.Setup(x => x.UpdateHotelImageAsync(hotelImage)).Returns(Task.CompletedTask);

            // Act
            await _hotelService.UpdateHotelImageAsync(hotelId, imageId, hotelImageDto);

            // Assert
            _mockHotelRepository.Verify(x => x.GetHotelImageByIdAsync(hotelId, imageId), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelImageDtoToHotelImage(hotelImageDto, hotelImage), Times.Once);
            _mockHotelRepository.Verify(x => x.UpdateHotelImageAsync(hotelImage), Times.Once);
        }

        [Fact]
        public async Task UpdateHotelImageAsync_WithInvalidHotelIdOrImageId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var imageId = _fixture.Create<int>();
            var hotelImageDto = _fixture.Create<HotelImageDto>();

            _mockHotelRepository.Setup(x => x.GetHotelImageByIdAsync(hotelId, imageId)).ReturnsAsync((HotelImage)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _hotelService.UpdateHotelImageAsync(hotelId, imageId, hotelImageDto));
            Assert.Equal($"Hotel Image with id {imageId} for hotel with id {hotelId} not found", exception.Message);

            _mockHotelRepository.Verify(x => x.GetHotelImageByIdAsync(hotelId, imageId), Times.Once);
            _mockHotelMapper.Verify(x => x.HotelImageDtoToHotelImage(It.IsAny<HotelImageDto>(), It.IsAny<HotelImage>()), Times.Never);
            _mockHotelRepository.Verify(x => x.UpdateHotelImageAsync(It.IsAny<HotelImage>()), Times.Never);
        }
    }
}
