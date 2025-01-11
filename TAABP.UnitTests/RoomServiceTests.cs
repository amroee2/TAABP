using AutoFixture;
using Moq;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.RoomMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.Services;
using TAABP.Core;

namespace TAABP.UnitTests
{
    public class RoomServiceTests
    {
        private readonly Mock<IRoomRepository> _roomRepository;
        private readonly Mock<IRoomMapper> _roomMapper;
        private readonly Mock<IHotelRepository> _hotelRepository;
        private readonly Mock<IUserService> _userService;
        private readonly RoomService _roomService;
        private readonly IFixture _fixture;

        public RoomServiceTests()
        {
            _roomRepository = new Mock<IRoomRepository>();
            _roomMapper = new Mock<IRoomMapper>();
            _hotelRepository = new Mock<IHotelRepository>();
            _userService = new Mock<IUserService>();

            _roomService = new RoomService(
                _roomRepository.Object,
                _roomMapper.Object,
                _hotelRepository.Object,
                _userService.Object
            );
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetRoomByIdAsync_WithValidHotelIdAndRoomId_ReturnsRoomDto()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var roomId = _fixture.Create<int>();
            _fixture.Customize<Room>(r =>
                r.With(h => h.HotelId, hotelId)
                 .With(h => h.RoomId, roomId));

            var room = _fixture.Create<Room>();
            var roomDto = _fixture.Create<RoomDto>();
            _hotelRepository.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync(_fixture.Create<Hotel>());
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomId)).ReturnsAsync(room);
            _roomMapper.Setup(x => x.RoomToRoomDto(room)).Returns(roomDto);

            // Act
            var result = await _roomService.GetRoomByIdAsync(hotelId, roomId);

            // Assert
            Assert.Equal(roomDto, result);
        }

        [Fact]
        public async Task GetRoomByIdAsync_WithInvalidHotelIdAndRoomId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var roomId = _fixture.Create<int>();
            _hotelRepository.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync((Hotel)null);
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomId)).ReturnsAsync((Room)null);

            // Act
            async Task Act() => await _roomService.GetRoomByIdAsync(hotelId, roomId);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        [Fact]
        public async Task GetRoomsAsync_WithValidHotelId_ReturnsListOfRoomDto()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var rooms = _fixture.CreateMany<Room>().ToList();
            var roomDtos = _fixture.CreateMany<RoomDto>().ToList();
            _hotelRepository.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync(_fixture.Create<Hotel>());
            _roomRepository.Setup(x => x.GetRoomsAsync(hotelId)).ReturnsAsync(rooms);
            foreach (var (room, roomDto) in rooms.Zip(roomDtos))
            {
                _roomMapper.Setup(x => x.RoomToRoomDto(room)).Returns(roomDto);
            }

            // Act
            var result = await _roomService.GetRoomsAsync(hotelId);

            // Assert
            Assert.Equal(roomDtos, result);
        }

        [Fact]
        public async Task GetRoomsAsync_WithInvalidHotelId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            _hotelRepository.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync((Hotel)null);

            // Act
            async Task Act() => await _roomService.GetRoomsAsync(hotelId);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        [Fact]
        public async Task CreateRoomAsync_WithValidRoomDto_ReturnsRoomId()
        {
            // Arrange
            var hotel = _fixture.Create<Hotel>();

            _fixture.Customize<RoomDto>(r => r.With(h => h.HotelId, hotel.HotelId));
            _fixture.Customize<Room>(r => r.With(h => h.HotelId, hotel.HotelId));

            var roomDto = _fixture.Create<RoomDto>();
            var room = _fixture.Create<Room>();
            var username = _fixture.Create<string>();

            _hotelRepository.Setup(x => x.GetHotelByIdAsync(roomDto.HotelId)).ReturnsAsync(hotel);
            _roomMapper.Setup(x => x.RoomDtoToRoom(roomDto, It.IsAny<Room>()))
                       .Callback<RoomDto, Room>((dto, r) =>
                       {
                           r.RoomId = room.RoomId;
                           r.HotelId = dto.HotelId;
                       });
            _userService.Setup(x => x.GetCurrentUsernameAsync()).ReturnsAsync(username);
            _roomRepository.Setup(x => x.CreateRoomAsync(It.IsAny<Room>())).Returns(Task.CompletedTask);

            // Act
            var result = await _roomService.CreateRoomAsync(roomDto);

            // Assert
            Assert.Equal(room.RoomId, result);
            _hotelRepository.Verify(x => x.GetHotelByIdAsync(roomDto.HotelId), Times.Once);
            _roomMapper.Verify(x => x.RoomDtoToRoom(roomDto, It.IsAny<Room>()), Times.Once);
            _userService.Verify(x => x.GetCurrentUsernameAsync(), Times.Once);
            _roomRepository.Verify(x => x.CreateRoomAsync(It.IsAny<Room>()), Times.Once);
        }


        [Fact]
        public async Task CreateRoomAsync_WithInvalidHotelId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var roomDto = _fixture.Create<RoomDto>();
            _hotelRepository.Setup(x => x.GetHotelByIdAsync(roomDto.HotelId)).ReturnsAsync((Hotel)null);

            // Act
            async Task Act() => await _roomService.CreateRoomAsync(roomDto);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        [Fact]
        public async Task UpdateRoomAsync_WithValidRoomDto_ReturnsRoomId()
        {
            // Arrange
            var hotel = _fixture.Create<Hotel>();
            _fixture.Customize<Room>(r => r.With(h => h.HotelId, hotel.HotelId));
            var room = _fixture.Create<Room>();
            _fixture.Customize<RoomDto>(r => r
                .With(h => h.HotelId, hotel.HotelId)
                .With(h => h.RoomId, room.RoomId));
            var roomDto = _fixture.Create<RoomDto>();
            var username = _fixture.Create<string>();
            _hotelRepository.Setup(x => x.GetHotelByIdAsync(roomDto.HotelId)).ReturnsAsync(hotel);
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomDto.RoomId)).ReturnsAsync(room);
            _roomMapper.Setup(x => x.RoomDtoToRoom(roomDto, room))
                       .Callback<RoomDto, Room>((dto, r) =>
                       {
                           r.HotelId = dto.HotelId;
                           r.RoomId = dto.RoomId;
                       });
            _userService.Setup(x => x.GetCurrentUsernameAsync()).ReturnsAsync(username);
            _roomRepository.Setup(x => x.UpdateRoomAsync(room)).Returns(Task.CompletedTask);

            // Act
            await _roomService.UpdateRoomAsync(roomDto);

            // Assert
            _roomRepository.Verify(x => x.UpdateRoomAsync(room), Times.Once);
            _hotelRepository.Verify(x => x.GetHotelByIdAsync(roomDto.HotelId), Times.Once);
            _roomRepository.Verify(x => x.GetRoomByIdAsync(roomDto.RoomId), Times.Once);
            _roomMapper.Verify(x => x.RoomDtoToRoom(roomDto, room), Times.Once);
            _userService.Verify(x => x.GetCurrentUsernameAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateRoomAsync_WithInvalidHotelIdAndRoomId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var roomDto = _fixture.Create<RoomDto>();
            _hotelRepository.Setup(x => x.GetHotelByIdAsync(roomDto.HotelId)).ReturnsAsync((Hotel)null);
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomDto.RoomId)).ReturnsAsync((Room)null);

            // Act
            async Task Act() => await _roomService.UpdateRoomAsync(roomDto);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        [Fact]
        public async Task DeleteRoomAsync_WithValidHotelIdAndRoomId_CallsDeleteRoomAsync()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var roomId = _fixture.Create<int>();
            var hotel = _fixture.Build<Hotel>()
                .With(h => h.HotelId, hotelId)
                .Create();

            var room = _fixture.Build<Room>()
                .With(r => r.RoomId, roomId)
                .With(r => r.HotelId, hotelId)
                .Create();

            _hotelRepository.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync(hotel);
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomId)).ReturnsAsync(room);
            _roomRepository.Setup(x => x.DeleteRoomAsync(It.IsAny<Room>())).Returns(Task.CompletedTask);

            // Act
            await _roomService.DeleteRoomAsync(hotelId, roomId);

            // Assert
            _roomRepository.Verify(x => x.DeleteRoomAsync(It.Is<Room>(r => r.RoomId == roomId && r.HotelId == hotelId)), Times.Once);
            _hotelRepository.Verify(x => x.GetHotelByIdAsync(hotelId), Times.Once);
            _roomRepository.Verify(x => x.GetRoomByIdAsync(roomId), Times.Once);
        }

        [Fact]
        public async Task DeleteRoomAsync_WithInvalidHotelIdAndRoomId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var hotelId = _fixture.Create<int>();
            var roomId = _fixture.Create<int>();
            _hotelRepository.Setup(x => x.GetHotelByIdAsync(hotelId)).ReturnsAsync((Hotel)null);
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomId)).ReturnsAsync((Room)null);

            // Act
            async Task Act() => await _roomService.DeleteRoomAsync(hotelId, roomId);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        [Fact]
        public async Task CreateRoomImageAsync_WithValidRoomImageDto_ReturnsRoomImageId()
        {
            // Arrange
            var roomImageDto = _fixture.Create<RoomImageDto>();
            var room = _fixture.Create<Room>();
            var roomImage = _fixture.Create<RoomImage>();
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomImageDto.RoomId)).ReturnsAsync(room);
            _roomMapper.Setup(x => x.RoomImageDtoToRoomImage(roomImageDto)).Returns(roomImage);
            _roomRepository.Setup(x => x.CreateRoomImageAsync(roomImage)).Returns(Task.CompletedTask);

            // Act
            var result = await _roomService.CreateRoomImageAsync(roomImageDto);

            // Assert
            Assert.Equal(roomImage.RoomImageId, result);
        }

        [Fact]
        public async Task CreateRoomImageAsync_WithInvalidRoomId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var roomImageDto = _fixture.Create<RoomImageDto>();
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomImageDto.RoomId)).ReturnsAsync((Room)null);

            // Act
            async Task Act() => await _roomService.CreateRoomImageAsync(roomImageDto);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        [Fact]
        public async Task GetRoomImageByIdAsync_WithValidRoomIdAndRoomImageId_ReturnsRoomImageDto()
        {
            // Arrange
            var roomId = _fixture.Create<int>();
            var roomImageId = _fixture.Create<int>();

            var room = _fixture.Build<Room>()
                .With(r => r.RoomId, roomId)
                .Create();

            var roomImage = _fixture.Build<RoomImage>()
                .With(ri => ri.RoomId, roomId)
                .With(ri => ri.RoomImageId, roomImageId)
                .Create();

            var roomImageDto = _fixture.Create<RoomImageDto>();

            _roomRepository.Setup(x => x.GetRoomImageByIdAsync(roomImageId)).ReturnsAsync(roomImage);
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomId)).ReturnsAsync(room);
            _roomMapper.Setup(x => x.RoomImageToRoomImageDto(roomImage)).Returns(roomImageDto);

            // Act
            var result = await _roomService.GetRoomImageByIdAsync(roomId, roomImageId);

            // Assert
            Assert.Equal(roomImageDto, result);
            _roomRepository.Verify(x => x.GetRoomImageByIdAsync(roomImageId), Times.Once);
            _roomRepository.Verify(x => x.GetRoomByIdAsync(roomId), Times.Once);
            _roomMapper.Verify(x => x.RoomImageToRoomImageDto(roomImage), Times.Once);
        }

        [Fact]
        public async Task GetRoomImageByIdAsync_WithInvalidRoomIdAndRoomImageId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var roomId = _fixture.Create<int>();
            var roomImageId = _fixture.Create<int>();
            _roomRepository.Setup(x => x.GetRoomImageByIdAsync(roomImageId)).ReturnsAsync((RoomImage)null);
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomId)).ReturnsAsync((Room)null);

            // Act
            async Task Act() => await _roomService.GetRoomImageByIdAsync(roomId, roomImageId);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        [Fact]
        public async Task GetRoomImagesAsync_WithValidRoomId_ReturnsListOfRoomImageDto()
        {
            // Arrange
            var roomId = _fixture.Create<int>();
            var room = _fixture.Create<Room>();
            var roomImages = _fixture.CreateMany<RoomImage>().ToList();
            var roomImageDtos = _fixture.CreateMany<RoomImageDto>().ToList();
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomId)).ReturnsAsync(room);
            _roomRepository.Setup(x => x.GetRoomImagesAsync(roomId)).ReturnsAsync(roomImages);
            foreach (var (roomImage, roomImageDto) in roomImages.Zip(roomImageDtos))
            {
                _roomMapper.Setup(x => x.RoomImageToRoomImageDto(roomImage)).Returns(roomImageDto);
            }

            // Act
            var result = await _roomService.GetRoomImagesAsync(roomId);

            // Assert
            Assert.Equal(roomImageDtos, result);
        }

        [Fact]
        public async Task GetRoomImagesAsync_WithInvalidRoomId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var roomId = _fixture.Create<int>();
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomId)).ReturnsAsync((Room)null);

            // Act
            async Task Act() => await _roomService.GetRoomImagesAsync(roomId);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }

        [Fact]
        public async Task DeleteRoomImageAsync_WithValidRoomIdAndRoomImageId_CallsDeleteRoomImageAsync()
        {
            // Arrange
            var roomId = _fixture.Create<int>();
            var roomImageId = _fixture.Create<int>();

            var room = _fixture.Build<Room>()
                .With(r => r.RoomId, roomId)
                .Create();

            var roomImage = _fixture.Build<RoomImage>()
                .With(ri => ri.RoomId, roomId)
                .With(ri => ri.RoomImageId, roomImageId)
                .Create();

            _roomRepository.Setup(x => x.GetRoomImageByIdAsync(roomImageId)).ReturnsAsync(roomImage);
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomId)).ReturnsAsync(room);

            // Act
            await _roomService.DeleteRoomImageAsync(roomId, roomImageId);

            // Assert
            _roomRepository.Verify(x => x.DeleteRoomImageAsync(It.Is<RoomImage>(ri => ri.RoomImageId == roomImageId)), Times.Once);
            _roomRepository.Verify(x => x.GetRoomImageByIdAsync(roomImageId), Times.Once);
            _roomRepository.Verify(x => x.GetRoomByIdAsync(roomId), Times.Once);
        }

        [Fact]
        public async Task DeleteRoomImageAsync_WithInvalidRoomIdAndRoomImageId_ThrowsEntityNotFoundException()
        {
            // Arrange
            var roomId = _fixture.Create<int>();
            var roomImageId = _fixture.Create<int>();
            _roomRepository.Setup(x => x.GetRoomImageByIdAsync(roomImageId)).ReturnsAsync((RoomImage)null);
            _roomRepository.Setup(x => x.GetRoomByIdAsync(roomId)).ReturnsAsync((Room)null);

            // Act
            async Task Act() => await _roomService.DeleteRoomImageAsync(roomId, roomImageId);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(Act);
        }
    }
}
