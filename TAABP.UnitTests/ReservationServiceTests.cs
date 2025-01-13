using AutoFixture;
using Moq;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.ReservationMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.Services;
using TAABP.Core;

namespace TAABP.UnitTests
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<IReservationMapper> _reservationMapperMock;
        private readonly Mock<IRoomRepository> _roomRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IHotelRepository> _hotelRepositoryMock;
        private readonly Mock<ICityRepository> _cityRepositoryMock;
        private readonly ReservationService _reservationService;
        private readonly Fixture _fixture;

        public ReservationServiceTests()
        {
            _reservationRepositoryMock = new Mock<IReservationRepository>();
            _reservationMapperMock = new Mock<IReservationMapper>();
            _roomRepositoryMock = new Mock<IRoomRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _hotelRepositoryMock = new Mock<IHotelRepository>();
            _cityRepositoryMock = new Mock<ICityRepository>();

            _reservationService = new ReservationService(
                _reservationRepositoryMock.Object,
                _reservationMapperMock.Object,
                _roomRepositoryMock.Object,
                _userRepositoryMock.Object,
                _hotelRepositoryMock.Object,
                _cityRepositoryMock.Object
            );

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetReservationByIdAsync_ShouldReturnReservation_WhenReservationExists()
        {
            // Arrange
            var userId = _fixture.Create<string>();
            var roomId = _fixture.Create<int>();
            var reservation = _fixture.Build<Reservation>()
                .With(r => r.UserId, userId)
                .With(r => r.RoomId, roomId)
                .Create();
            var reservationDto = _fixture.Create<ReservationDto>();

            _reservationRepositoryMock.Setup(repo => repo.GetReservationByIdAsync(reservation.ReservationId))
                .ReturnsAsync(reservation);
            _reservationMapperMock.Setup(mapper => mapper.ReservationToResevationDto(reservation))
                .Returns(reservationDto);
            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(reservation.RoomId))
                .ReturnsAsync(new Room { RoomId = reservation.RoomId });
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(reservation.UserId))
                .ReturnsAsync(new User { Id = reservation.UserId });

            // Act
            var result = await _reservationService.GetReservationByIdAsync(userId, roomId, reservation.ReservationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reservationDto, result);
        }


        [Fact]
        public async Task GetReservationByIdAsync_ShouldThrowException_WhenReservationDoesNotExist()
        {
            // Arrange
            var reservationId = _fixture.Create<int>();
            var userId = _fixture.Create<string>();
            var roomId = _fixture.Create<int>();
            _reservationRepositoryMock.Setup(repo => repo.GetReservationByIdAsync(reservationId))
                .ReturnsAsync((Reservation)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _reservationService.GetReservationByIdAsync(userId, roomId, reservationId));
        }

        [Fact]
        public async Task GetReservationsAsync_ShouldReturnListOfReservations()
        {
            // Arrange
            var reservations = _fixture.CreateMany<Reservation>().ToList();
            var reservationDtos = _fixture.CreateMany<ReservationDto>().ToList();

            _reservationRepositoryMock.Setup(repo => repo.GetReservationsAsync()).ReturnsAsync(reservations);
            _reservationMapperMock.Setup(mapper => mapper.ReservationToResevationDto(It.IsAny<Reservation>()))
                .Returns((Reservation r) => reservationDtos.FirstOrDefault(dto => dto.ReservationId == r.ReservationId));

            // Act
            var result = await _reservationService.GetReservationsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reservations.Count, result.Count);
        }

        [Fact]
        public async Task CreateReservationAsync_ShouldCreateReservation_WhenAllConditionsAreMet()
        {
            // Arrange
            var now = DateTime.Now;
            var reservationDto = new ReservationDto
            {
                RoomId =_fixture.Create<int>(),
                UserId = _fixture.Create<string>(),
                StartDate = now.AddDays(3),
                EndDate = now.AddDays(5)
            };

            var room = new Room
            {
                RoomId = reservationDto.RoomId,
                IsAvailable = true,
                PricePerNight = 100,
                HotelId = _fixture.Create<int>()
            };

            var user = new User { Id = reservationDto.UserId };
            var hotel = new Hotel { HotelId = room.HotelId, CityId = _fixture.Create<int>() };

            var reservation = new Reservation
            {
                ReservationId = _fixture.Create<int>(),
                StartDate = reservationDto.StartDate,
                EndDate = reservationDto.EndDate
            };

            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(reservationDto.RoomId)).ReturnsAsync(room);
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(reservationDto.UserId)).ReturnsAsync(user);
            _hotelRepositoryMock.Setup(repo => repo.GetHotelByIdAsync(room.HotelId)).ReturnsAsync(hotel);
            _reservationMapperMock.Setup(mapper => mapper.ReservationDtoToReservation(reservationDto)).Returns(reservation);

            // Act
            var result = await _reservationService.CreateReservationAsync(reservationDto);

            // Assert
            Assert.Equal(reservation.ReservationId, result);
            Assert.Equal(200, reservation.Price);
            _roomRepositoryMock.Verify(repo => repo.BookRoomAsync(room.RoomId), Times.Once);
            _hotelRepositoryMock.Verify(repo => repo.IncrementNumberOfVisitsAsync(room.HotelId), Times.Once);
            _cityRepositoryMock.Verify(repo => repo.IncrementNumberOfVisitsAsync(hotel.CityId), Times.Once);
        }


        [Fact]
        public async Task CreateReservationAsync_ShouldThrowException_WhenRoomOrUserDoesNotExist()
        {
            // Arrange
            var reservationDto = _fixture.Create<ReservationDto>();

            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(reservationDto.RoomId)).ReturnsAsync((Room)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _reservationService.CreateReservationAsync(reservationDto));
        }

        [Fact]
        public async Task CreateReservationAsync_ShouldThrowException_WhenRoomIsNotAvailable()
        {
            // Arrange
            var reservationDto = new ReservationDto
            {
                RoomId = _fixture.Create<int>(),
                UserId = _fixture.Create<string>(),
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(5)
            };

            var room = new Room
            {
                RoomId = reservationDto.RoomId,
                IsAvailable = false
            };

            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(reservationDto.RoomId)).ReturnsAsync(room);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _reservationService.CreateReservationAsync(reservationDto));
        }

        [Fact]
        public async Task UpdateReservationAsync_ShouldUpdateReservation_WhenConditionsAreMet()
        {
            // Arrange
            var now = DateTime.Now;
            var reservationDto = new ReservationDto
            {
                ReservationId = _fixture.Create<int>(),
                RoomId = _fixture.Create<int>(),
                StartDate = now.AddDays(3),
                EndDate = now.AddDays(5)
            };

            var targetReservation = new Reservation
            {
                ReservationId = reservationDto.ReservationId,
                RoomId = reservationDto.RoomId,
                StartDate = now.AddDays(3),
                EndDate = now.AddDays(5),
                Price = 200
            };

            var room = new Room
            {
                RoomId = reservationDto.RoomId,
                PricePerNight = 100
            };

            _reservationRepositoryMock.Setup(repo => repo.GetReservationByIdAsync(reservationDto.ReservationId))
                .ReturnsAsync(targetReservation);

            _reservationMapperMock.Setup(mapper => mapper.ReservationDtoToReservation(reservationDto))
                .Returns(new Reservation
                {
                    ReservationId = reservationDto.ReservationId,
                    RoomId = reservationDto.RoomId,
                    StartDate = reservationDto.StartDate,
                    EndDate = reservationDto.EndDate
                });

            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(reservationDto.RoomId))
                .ReturnsAsync(room);

            // Act
            await _reservationService.UpdateReservationAsync(reservationDto);

            // Assert
            _reservationRepositoryMock.Verify(repo => repo.UpdateReservationAsync(It.Is<Reservation>(r =>
                r.ReservationId == reservationDto.ReservationId &&
                r.RoomId == reservationDto.RoomId &&
                r.StartDate == reservationDto.StartDate &&
                r.EndDate == reservationDto.EndDate &&
                r.Price == 200
            )), Times.Once);
        }


        [Fact]
        public async Task UpdateReservationAsync_ShouldThrowException_WhenReservationDoesNotExist()
        {
            // Arrange
            var reservationDto = _fixture.Create<ReservationDto>();

            _reservationRepositoryMock.Setup(repo => repo.GetReservationByIdAsync(reservationDto.ReservationId))
                .ReturnsAsync((Reservation)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _reservationService.UpdateReservationAsync(reservationDto));
        }

        [Fact]
        public async Task DeleteReservationAsync_ShouldDeleteReservation_WhenConditionsAreMet()
        {
            // Arrange
            _fixture.Customize<Reservation>(r => r.With(r => r.StartDate, DateTime.Now.AddDays(3)).With(r => r.EndDate, DateTime.Now.AddDays(5)));
            var reservationId = _fixture.Create<int>();
            var targetReservation = _fixture.Create<Reservation>();
            var room = _fixture.Create<Room>();
            _reservationRepositoryMock.Setup(repo => repo.GetReservationByIdAsync(reservationId))
                .ReturnsAsync(targetReservation);
            _roomRepositoryMock.Setup(repo => repo.GetRoomByIdAsync(targetReservation.RoomId))
                .ReturnsAsync(room);
            _roomRepositoryMock.Setup(repo => repo.UnbookRoomAsync(room.RoomId));
            var hotel = _fixture.Create<Hotel>();
            _hotelRepositoryMock.Setup(repo => repo.GetHotelByIdAsync(room.HotelId))
                .ReturnsAsync(hotel);
            _hotelRepositoryMock.Setup(repo => repo.DecrementNumberOfVisitsAsync(hotel.HotelId));
            _cityRepositoryMock.Setup(repo => repo.DecrementNumberOfVisitsAsync(hotel.CityId));
            // Act
            await _reservationService.DeleteReservationAsync(reservationId);

            // Assert
            _reservationRepositoryMock.Verify(repo => repo.DeleteReservationAsync(targetReservation), Times.Once);
        }

        [Fact]
        public async Task DeleteReservationAsync_ShouldThrowException_WhenReservationDoesNotExist()
        {
            // Arrange
            var reservationId = _fixture.Create<int>();

            _reservationRepositoryMock.Setup(repo => repo.GetReservationByIdAsync(reservationId))
                .ReturnsAsync((Reservation)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _reservationService.DeleteReservationAsync(reservationId));
        }
    }
}
