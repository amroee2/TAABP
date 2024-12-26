using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.ReservationMapping;
using TAABP.Application.RepositoryInterfaces;

namespace TAABP.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IReservationMapper _reservationMapper;
        private readonly IRoomRepository _roomRepository;
        private readonly IUserRepository _userRepository;

        public ReservationService(IReservationRepository reservationRepository, IReservationMapper reservationMapper, 
            IRoomRepository roomRepository, IUserRepository userRepository)
        {
            _reservationRepository = reservationRepository;
            _reservationMapper = reservationMapper;
            _roomRepository = roomRepository;
            _userRepository = userRepository;
        }

        public async Task<ReservationDto> GetReservationByIdAsync(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            return _reservationMapper.ReservationToResevationDto(reservation);
        }

        public async Task<List<ReservationDto>> GetReservationsAsync()
        {
            var reservations = await _reservationRepository.GetReservationsAsync();
            return reservations.Select(r => _reservationMapper.ReservationToResevationDto(r)).ToList();
        }

        public async Task CreateReservationAsync(ReservationDto reservationDto)
        {
            var room = await _roomRepository.GetRoomByIdAsync(reservationDto.RoomId);
            var user = await _userRepository.GetUserByIdAsync(reservationDto.UserId);
            if (room == null || user == null)
            {
                throw new EntityNotFoundException("Room or user not found");
            }
            var reservation = _reservationMapper.ReservationDtoToReservation(reservationDto);
            var price = (reservation.EndDate - reservation.StartDate).Days * room.PricePerNight;
            reservation.Price = price;
            await _reservationRepository.CreateReservationAsync(reservation);
        }

        public async Task UpdateReservationAsync(ReservationDto reservationDto)
        {
            var targetReservation = await _reservationRepository.GetReservationByIdAsync(reservationDto.ReservationId);
            if (targetReservation == null)
            {
                throw new EntityNotFoundException("Reservation not found");
            }
            var reservation = _reservationMapper.ReservationDtoToReservation(reservationDto);
            await _reservationRepository.UpdateReservationAsync(reservation);
        }

        public async Task DeleteReservationAsync(int reservationId)
        {
            var targetReservation = await _reservationRepository.GetReservationByIdAsync(reservationId);
            if (targetReservation == null)
            {
                throw new EntityNotFoundException("Reservation not found");
            }
            await _reservationRepository.DeleteReservationAsync(targetReservation);
        }
    }
}
