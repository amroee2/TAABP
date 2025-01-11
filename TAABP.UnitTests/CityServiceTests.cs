using AutoFixture;
using Moq;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.CityMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.Services;
using TAABP.Core;

namespace TAABP.UnitTests
{
    public class CityServiceTests
    {
        private readonly CityService _cityService;
        private readonly Mock<ICityRepository> _cityRepositoryMock;
        private readonly Mock<ICityMapper> _cityMapperMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly IFixture _fixture;

        public CityServiceTests()
        {
            _cityRepositoryMock = new Mock<ICityRepository>();
            _cityMapperMock = new Mock<ICityMapper>();
            _userServiceMock = new Mock<IUserService>();
            _cityService = new CityService(_cityRepositoryMock.Object, _cityMapperMock.Object, _userServiceMock.Object);
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetCitiesAsync_ShouldReturnListOfCities()
        {
            // Arrange
            var cities = _fixture.Create<List<City>>();
            var cityDtos = cities.Select(city => _fixture.Create<CityDto>()).ToList();
            _cityRepositoryMock.Setup(x => x.GetCitiesAsync()).ReturnsAsync(cities);
            _cityMapperMock.Setup(x => x.CityToCityDto(It.IsAny<City>())).Returns(cityDtos.First());

            // Act
            var result = await _cityService.GetCitiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cities.Count, result.Count);
        }

        [Fact]
        public async Task GetCitiesAsync_ShouldReturnEmptyList()
        {
            // Arrange
            _cityRepositoryMock.Setup(x => x.GetCitiesAsync()).ReturnsAsync(new List<City>());

            // Act
            var result = await _cityService.GetCitiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCityByIdAsync_ShouldReturnCity()
        {
            // Arrange
            var city = _fixture.Create<City>();
            var cityDto = _fixture.Create<CityDto>();
            _cityRepositoryMock.Setup(x => x.GetCityByIdAsync(It.IsAny<int>())).ReturnsAsync(city);
            _cityMapperMock.Setup(x => x.CityToCityDto(It.IsAny<City>())).Returns(cityDto);

            // Act
            var result = await _cityService.GetCityByIdAsync(city.CityId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cityDto.CityId, result.CityId);
        }

        [Fact]
        public async Task GetCityByIdAsync_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            _cityRepositoryMock.Setup(x => x.GetCityByIdAsync(It.IsAny<int>())).ReturnsAsync((City)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _cityService.GetCityByIdAsync(1));
        }

        [Fact]
        public async Task CreateCityAsync_ShouldReturnCityId()
        {
            // Arrange
            int id = _fixture.Create<int>();
            var cityDto = _fixture.Build<CityDto>().With(c => c.CityId, id).Create();
            var city = _fixture.Build<City>().With(c => c.CityId, id).Create();

            _cityMapperMock.Setup(x => x.CityDtoToCity(It.IsAny<CityDto>(), It.IsAny<City>()))
                .Callback<CityDto, City>((dto, c) =>
                {
                    c.CityId = dto.CityId;
                    c.CreatedAt = DateTime.Now;
                });

            _userServiceMock.Setup(x => x.GetCurrentUsernameAsync()).ReturnsAsync("TestUser");
            _cityRepositoryMock.Setup(x => x.CreateCityAsync(It.IsAny<City>())).Returns(Task.CompletedTask);

            // Act
            var result = await _cityService.CreateCityAsync(cityDto);

            // Assert
            Assert.Equal(id, result);
        }

        [Fact]
        public async Task UpdateCityAsync_ShouldUpdateCity()
        {
            // Arrange
            var cityDto = _fixture.Create<CityDto>();
            var city = _fixture.Create<City>();
            _cityRepositoryMock.Setup(x => x.GetCityByIdAsync(It.IsAny<int>())).ReturnsAsync(city);
            _cityMapperMock.Setup(x => x.CityDtoToCity(It.IsAny<CityDto>(), It.IsAny<City>()));
            _userServiceMock.Setup(x => x.GetCurrentUsernameAsync()).ReturnsAsync(city.UpdatedBy);
            _cityRepositoryMock.Setup(x => x.UpdateCityAsync(It.IsAny<City>()));

            // Act
            await _cityService.UpdateCityAsync(cityDto);

            // Assert
            _cityRepositoryMock.Verify(x => x.UpdateCityAsync(It.IsAny<City>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCityAsync_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            _cityRepositoryMock.Setup(x => x.GetCityByIdAsync(It.IsAny<int>())).ReturnsAsync((City)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _cityService.UpdateCityAsync(_fixture.Create<CityDto>()));
        }

        [Fact]
        public async Task DeleteCityAsync_ShouldDeleteCity()
        {
            // Arrange
            var city = _fixture.Create<City>();
            _cityRepositoryMock.Setup(x => x.GetCityByIdAsync(It.IsAny<int>())).ReturnsAsync(city);
            _cityRepositoryMock.Setup(x => x.DeleteCityAsync(It.IsAny<City>()));

            // Act
            await _cityService.DeleteCityAsync(city.CityId);

            // Assert
            _cityRepositoryMock.Verify(x => x.DeleteCityAsync(It.IsAny<City>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCityAsync_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            _cityRepositoryMock.Setup(x => x.GetCityByIdAsync(It.IsAny<int>())).ReturnsAsync((City)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _cityService.DeleteCityAsync(1));
        }

        [Fact]
        public async Task GetTopCitiesAsync_ShouldReturnListOfCities()
        {
            // Arrange
            var cities = _fixture.Create<List<City>>();
            _cityRepositoryMock.Setup(x => x.GetTopCitiesAsync()).ReturnsAsync(cities);

            // Act
            var result = await _cityService.GetTopCitiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cities.Count, result.Count);
        } 
    }
}
