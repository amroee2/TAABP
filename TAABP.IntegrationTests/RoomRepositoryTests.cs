using AutoFixture;
using Microsoft.EntityFrameworkCore;
using TAABP.Core;
using TAABP.Infrastructure;
using TAABP.Infrastructure.Repositories;

namespace TAABP.IntegrationTests
{
    public class RoomRepositoryTests
    {
        private readonly TAABPDbContext _context;
        private readonly RoomRepository _roomRepository;
        private readonly IFixture _fixture;

        public RoomRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TAABPDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TAABPDbContext(options);

            _roomRepository = new RoomRepository(_context);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task CreateRoomAsync_ShouldAddRoomAsync()
        {
            // Arrange
            var room = _fixture.Create<Room>();

            // Act
            await _roomRepository.CreateRoomAsync(room);

            // Assert
            var result = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == room.RoomId);
            Assert.Equal(room, result);
        }

        [Fact]
        public async Task GetRoomByIdAsync_ShouldReturnRoomByIdAsync()
        {
            // Arrange
            var room = _fixture.Create<Room>();
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            // Act
            var result = await _roomRepository.GetRoomByIdAsync(room.RoomId);

            // Assert
            Assert.Equal(room.RoomId, result.RoomId);
        }

        [Fact]
        public async Task GetRoomsAsync_ShouldReturnAllRoomsAsync()
        {
            // Arrange
            var hotel = _fixture.Create<Hotel>();
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();
            var rooms = _fixture.CreateMany<Room>().ToList();
            await _context.Rooms.AddRangeAsync(rooms);
            await _context.SaveChangesAsync();

            // Act
            var result = await _roomRepository.GetRoomsAsync(hotel.HotelId);

            // Assert
            Assert.Equal(rooms.Count, result.Count);
        }

        [Fact]
        public async Task DeleteRoomAsync_ShouldDeleteRoomAsync()
        {
            // Arrange
            var room = _fixture.Create<Room>();
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            // Act
            await _roomRepository.DeleteRoomAsync(room);

            // Assert
            var result = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == room.RoomId);
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateRoomAsync_ShouldUpdateRoomAsync()
        {
            // Arrange
            var room = _fixture.Create<Room>();
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            // Act
            room.RoomNumber = 10;
            await _roomRepository.UpdateRoomAsync(room);

            // Assert
            var result = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == room.RoomId);
            Assert.Equal(10, result.RoomNumber);
        }

        [Fact]
        public async Task CreateNewRoomImageAsync_ShouldAddRoomImageAsync()
        {
            // Arrange
            var roomImage = _fixture.Create<RoomImage>();

            // Act
            await _roomRepository.CreateRoomImageAsync(roomImage);

            // Assert
            var result = await _context.RoomImages.FirstOrDefaultAsync(r => r.RoomId == roomImage.RoomId && r.RoomImageId == roomImage.RoomImageId);
            Assert.Equal(roomImage, result);
        }

        [Fact]
        public async Task GetRoomImageByIdAsync_ShouldReturnRoomImageByIdAsync()
        {
            // Arrange
            var roomImage = _fixture.Create<RoomImage>();
            await _context.RoomImages.AddAsync(roomImage);
            await _context.SaveChangesAsync();

            // Act
            var result = await _roomRepository.GetRoomImageByIdAsync(roomImage.RoomImageId);

            // Assert
            Assert.Equal(roomImage.RoomImageId, result.RoomImageId);
        }

        [Fact]
        public async Task GetRoomImagesAsync_ShouldReturnAllRoomImagesAsync()
        {
            // Arrange
            var room = _fixture.Create<Room>();
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            _fixture.Customize<RoomImage>(c => c.With(r=>r.RoomId, room.RoomId));

            var roomImages = _fixture.CreateMany<RoomImage>().ToList();
            await _context.RoomImages.AddRangeAsync(roomImages);
            await _context.SaveChangesAsync();

            // Act
            var result = await _roomRepository.GetRoomImagesAsync(room.RoomId);

            // Assert
            Assert.Equal(roomImages.Count, result.Count);
            Assert.All(result, img => Assert.Equal(room.RoomId, img.RoomId));

        }

        [Fact]
        public async Task DeleteRoomImageAsync_ShouldDeleteRoomImageAsync()
        {
            // Arrange
            var roomImage = _fixture.Create<RoomImage>();
            await _context.RoomImages.AddAsync(roomImage);
            await _context.SaveChangesAsync();

            // Act
            await _roomRepository.DeleteRoomImageAsync(roomImage);

            // Assert
            var result = await _context.RoomImages.FirstOrDefaultAsync(r => r.RoomId == roomImage.RoomId && r.RoomImageId == roomImage.RoomImageId);
            Assert.Null(result);
        }

        [Fact]
        public async Task BookRoomAsync_ShouldBookRoomAsync()
        {
            // Arrange
            var room = _fixture.Create<Room>();
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            // Act
            await _roomRepository.BookRoomAsync(room.RoomId);

            // Assert
            var result = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == room.RoomId);
            Assert.False(result.IsAvailable);
        }

        [Fact]
        public async Task UnbookRoomAsync_ShouldUnbookRoomAsync()
        {
            // Arrange
            var room = _fixture.Create<Room>();
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            // Act
            await _roomRepository.UnbookRoomAsync(room.RoomId);

            // Assert
            var result = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == room.RoomId);
            Assert.True(result.IsAvailable);
        }
    }
}
