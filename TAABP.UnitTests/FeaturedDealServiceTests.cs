using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.FeaturedDealMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.Services;
using TAABP.Core;

namespace TAABP.UnitTests
{
    public class FeaturedDealServiceTests
    {
        private readonly Mock<IFeaturedDealRepository> _featuredDealRepositoryMock;
        private readonly Mock<IFeaturedDealMapper> _featuredDealMapperMock;
        private readonly Mock<IRoomRepository> _roomRepositoryMock;
        private readonly FeaturedDealService _featuredDealService;
        private readonly IFixture _fixture;

        public FeaturedDealServiceTests()
        {
            _featuredDealRepositoryMock = new Mock<IFeaturedDealRepository>();
            _featuredDealMapperMock = new Mock<IFeaturedDealMapper>();
            _roomRepositoryMock = new Mock<IRoomRepository>();
            _featuredDealService = new FeaturedDealService(
                _roomRepositoryMock.Object,
                _featuredDealMapperMock.Object,
                _featuredDealRepositoryMock.Object);

            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task UpdateFeaturedDealAsync_ShouldUpdate_WhenFeaturedDealAndRoomExist()
        {
            // Arrange
            var roomId = _fixture.Create<int>();
            var featuredDealId = _fixture.Create<int>();

            var room = _fixture.Build<Room>()
                .With(r => r.RoomId, roomId)
                .Create();

            var featuredDealDto = _fixture.Build<FeatueredDealDto>()
                .With(fd => fd.FeaturedDealId, featuredDealId)
                .With(fd => fd.RoomId, roomId)
                .Create();

            var existingFeaturedDeal = _fixture.Build<FeaturedDeal>()
                .With(fd => fd.FeaturedDealId, featuredDealId)
                .With(fd => fd.RoomId, roomId)
                .Create();

            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(roomId)).ReturnsAsync(room);
            _featuredDealRepositoryMock.Setup(repo => repo.GetFeaturedDealAsync(featuredDealId)).ReturnsAsync(existingFeaturedDeal);
            _featuredDealRepositoryMock.Setup(repo => repo.UpdateFeaturedDealAsync(It.IsAny<FeaturedDeal>()))
                .Returns(Task.CompletedTask);
            _featuredDealMapperMock.Setup(mapper => mapper.FeaturedDealDtoToFeaturedDeal(featuredDealDto))
                .Returns(new FeaturedDeal
                {
                    FeaturedDealId = featuredDealDto.FeaturedDealId,
                    RoomId = featuredDealDto.RoomId
                });


            // Act
            await _featuredDealService.UpdateFeaturedDealAsync(featuredDealDto);

            // Assert
            _featuredDealMapperMock.Verify(mapper => mapper.FeaturedDealDtoToFeaturedDeal(featuredDealDto), Times.Once);
            _featuredDealRepositoryMock.Verify(repo => repo.UpdateFeaturedDealAsync(It.Is<FeaturedDeal>(fd =>
                fd.FeaturedDealId == featuredDealDto.FeaturedDealId &&
                fd.RoomId == roomId)), Times.Once);
        }


        [Fact]
        public async Task UpdateFeaturedDealAsync_ShouldThrowException_WhenRoomDoesNotExist()
        {
            // Arrange
            var featuredDealDto = _fixture.Create<FeatueredDealDto>();

            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(featuredDealDto.RoomId)).ReturnsAsync((Room)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _featuredDealService.UpdateFeaturedDealAsync(featuredDealDto));
        }

        [Fact]
        public async Task UpdateFeaturedDealAsync_ShouldThrowException_WhenFeaturedDealDoesNotExist()
        {
            // Arrange
            var featuredDealDto = _fixture.Create<FeatueredDealDto>();
            var room = _fixture.Create<Room>();

            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(featuredDealDto.RoomId)).ReturnsAsync(room);
            _featuredDealRepositoryMock.Setup(repo => repo.GetFeaturedDealAsync(featuredDealDto.FeaturedDealId)).ReturnsAsync((FeaturedDeal)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _featuredDealService.UpdateFeaturedDealAsync(featuredDealDto));
        }

        [Fact]
        public async Task DeleteFeaturedDealAsync_ShouldDelete_WhenFeaturedDealExists()
        {
            // Arrange
            var featuredDealId = _fixture.Create<int>();
            var featuredDeal = _fixture.Create<FeaturedDeal>();

            _featuredDealRepositoryMock.Setup(repo => repo.GetFeaturedDealAsync(featuredDealId)).ReturnsAsync(featuredDeal);

            // Act
            await _featuredDealService.DeleteFeaturedDealAsync(featuredDealId);

            // Assert
            _featuredDealRepositoryMock.Verify(repo => repo.DeleteFeaturedDealAsync(featuredDeal), Times.Once);
        }

        [Fact]
        public async Task DeleteFeaturedDealAsync_ShouldThrowException_WhenFeaturedDealDoesNotExist()
        {
            // Arrange
            var featuredDealId = _fixture.Create<int>();

            _featuredDealRepositoryMock.Setup(repo => repo.GetFeaturedDealAsync(featuredDealId)).ReturnsAsync((FeaturedDeal)null);
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _featuredDealService.DeleteFeaturedDealAsync(featuredDealId));
        }


        [Fact]
        public async Task GetFeaturedDealByIdAsync_ShouldReturnDto_WhenFeaturedDealExists()
        {
            // Arrange
            var featuredDealId = _fixture.Create<int>();
            var featuredDeal = _fixture.Create<FeaturedDeal>();
            var featuredDealDto = _fixture.Create<FeatueredDealDto>();

            _featuredDealRepositoryMock.Setup(repo => repo.GetFeaturedDealAsync(featuredDealId))
                .ReturnsAsync(featuredDeal);
            _featuredDealMapperMock.Setup(mapper => mapper.FeaturedDealToFeaturedDealDto(featuredDeal))
                .Returns(featuredDealDto);

            // Act
            var result = await _featuredDealService.GetFeaturedDealByIdAsync(featuredDealId);

            // Assert
            Assert.Equal(featuredDealDto, result);
            _featuredDealRepositoryMock.Verify(repo => repo.GetFeaturedDealAsync(featuredDealId), Times.Once);
            _featuredDealMapperMock.Verify(mapper => mapper.FeaturedDealToFeaturedDealDto(featuredDeal), Times.Once);
        }

        [Fact]
        public async Task GetFeaturedDealByIdAsync_ShouldThrowException_WhenFeaturedDealDoesNotExist()
        {
            // Arrange
            var featuredDealId = _fixture.Create<int>();

            _featuredDealRepositoryMock.Setup(repo => repo.GetFeaturedDealAsync(featuredDealId))
                .ReturnsAsync((FeaturedDeal)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _featuredDealService.GetFeaturedDealByIdAsync(featuredDealId));
        }

        [Fact]
        public async Task GetFeaturedDealsAsync_ShouldReturnListOfDtos()
        {
            // Arrange
            var featuredDeals = _fixture.CreateMany<FeaturedDeal>().ToList();
            var featuredDealsDto = _fixture.CreateMany<FeatueredDealDto>().ToList();

            _featuredDealRepositoryMock.Setup(repo => repo.GetFeaturedDealsAsync())
                .ReturnsAsync(featuredDeals);
            _featuredDealMapperMock.Setup(mapper => mapper.FeaturedDealToFeaturedDealDto(It.IsAny<FeaturedDeal>()))
                .Returns((FeaturedDeal fd) => featuredDealsDto.FirstOrDefault(dto => dto.FeaturedDealId == fd.FeaturedDealId));

            // Act
            var result = await _featuredDealService.GetFeaturedDealsAsync();

            // Assert
            Assert.Equal(featuredDealsDto.Count, result.Count);
            _featuredDealRepositoryMock.Verify(repo => repo.GetFeaturedDealsAsync(), Times.Once);
            _featuredDealMapperMock.Verify(mapper => mapper.FeaturedDealToFeaturedDealDto(It.IsAny<FeaturedDeal>()), Times.Exactly(featuredDeals.Count));
        }

        [Fact]
        public async Task CreateFeaturedDealAsync_ShouldCreate_WhenRoomExists()
        {
            // Arrange
            var featuredDealDto = _fixture.Create<FeatueredDealDto>();
            var room = _fixture.Create<Room>();
            var featuredDeal = _fixture.Create<FeaturedDeal>();

            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(featuredDealDto.RoomId))
                .ReturnsAsync(room);
            _featuredDealMapperMock.Setup(mapper => mapper.FeaturedDealDtoToFeaturedDeal(featuredDealDto))
                .Returns(featuredDeal);

            // Act
            var id = await _featuredDealService.CreateFeaturedDealAsync(featuredDealDto);

            // Assert
            Assert.Equal(featuredDeal.FeaturedDealId, id);
            _roomRepositoryMock.Verify(repo => repo.GetRoomByIdAsync(featuredDealDto.RoomId), Times.Once);
            _featuredDealMapperMock.Verify(mapper => mapper.FeaturedDealDtoToFeaturedDeal(featuredDealDto), Times.Once);
            _featuredDealRepositoryMock.Verify(repo => repo.CreateFeaturedDealAsync(featuredDeal), Times.Once);
        }

        [Fact]
        public async Task CreateFeaturedDealAsync_ShouldThrowException_WhenRoomDoesNotExist()
        {
            // Arrange
            var featuredDealDto = _fixture.Create<FeatueredDealDto>();

            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(featuredDealDto.RoomId))
                .ReturnsAsync((Room)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _featuredDealService.CreateFeaturedDealAsync(featuredDealDto));
        }
    }
}