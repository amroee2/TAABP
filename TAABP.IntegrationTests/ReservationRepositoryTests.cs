using AutoFixture;
using Microsoft.EntityFrameworkCore;
using TAABP.Core;
using TAABP.Infrastructure;
using TAABP.Infrastructure.Repositories;

namespace TAABP.IntegrationTests
{
    public class ReservationRepositoryTests
    {
        private readonly TAABPDbContext _context;
        private readonly ReservationRepository _reservationRepository;
        private readonly IFixture _fixture;

        public ReservationRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TAABPDbContext>()
             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
             .Options;
            _context = new TAABPDbContext(options);

            _reservationRepository = new ReservationRepository(_context);

            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task CreateReservationAsync_ShouldAddReservationAsync()
        {
            // Arrange
            var reservation = _fixture.Create<Reservation>();

            // Act
            await _reservationRepository.CreateReservationAsync(reservation);

            // Assert
            var result = await _context.Reservations.FirstOrDefaultAsync(r => r.ReservationId == reservation.ReservationId);
            Assert.Equal(reservation, result);
        }

        [Fact]
        public async Task GetReservationByIdAsync_ShouldReturnReservationByIdAsync()
        {
            // Arrange
            var reservation = _fixture.Create<Reservation>();
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _reservationRepository.GetReservationByIdAsync(reservation.ReservationId);

            // Assert
            Assert.Equal(reservation.ReservationId, result.ReservationId);
        }

        [Fact]
        public async Task GetReservationsAsync_ShouldReturnAllReservationsAsync()
        {
            // Arrange
            var user = _fixture.Create<User>();
            await _context.AddAsync(user);
            _context.SaveChanges();
            _fixture.Customize<Reservation>(c => c.With(r => r.UserId, user.Id));
            var reservations = _fixture.CreateMany<Reservation>(3).ToList();
            await _context.Reservations.AddRangeAsync(reservations);
            await _context.SaveChangesAsync();

            // Act
            var result = await _reservationRepository.GetReservationsAsync(user.Id);

            // Assert
            Assert.Equal(reservations.Count, result.Count);
        }

        [Fact]
        public async Task DeleteReservationAsync_ShouldDeleteReservationAsync()
        {
            // Arrange
            var reservation = _fixture.Create<Reservation>();
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            // Act
            await _reservationRepository.DeleteReservationAsync(reservation);

            // Assert
            var result = await _context.Reservations.FirstOrDefaultAsync(r => r.ReservationId == reservation.ReservationId);
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateReservationAsync_ShouldUpdateReservationAsync()
        {
            // Arrange
            var reservation = _fixture.Create<Reservation>();
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            // Act
            reservation.RoomId = 2;
            await _reservationRepository.UpdateReservationAsync(reservation);

            // Assert
            var result = await _context.Reservations.FirstOrDefaultAsync(r => r.ReservationId == reservation.ReservationId);
            Assert.Equal(reservation.RoomId, result.RoomId);
        }
    }
}
