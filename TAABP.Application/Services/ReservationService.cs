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
        private readonly IHotelRepository _hotelRepository;
        private readonly ICityRepository _cityRepository;

        public ReservationService(IReservationRepository reservationRepository, IReservationMapper reservationMapper, 
            IRoomRepository roomRepository, IUserRepository userRepository, IHotelRepository hotelRepository,
            ICityRepository cityRepository)
        {
            _reservationRepository = reservationRepository;
            _reservationMapper = reservationMapper;
            _roomRepository = roomRepository;
            _userRepository = userRepository;
            _hotelRepository = hotelRepository;
            _cityRepository = cityRepository;
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
            room.IsAvailable = false;
            var hotel = await _hotelRepository.GetHotelAsync(room.HotelId);
            var city = await _cityRepository.GetCityByIdAsync(hotel.CityId);
            hotel.NumberOfVisits++;
            city.NumberOfVisits++;
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
            if (DateTime.Now >= targetReservation.StartDate.AddHours(-24) || DateTime.Now >= targetReservation.EndDate)
            {
                throw new InvalidOperationException("Reservations cannot be updated within 24 hours of the start date, during the stay, or after completion.");
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
            if (DateTime.Now >= targetReservation.StartDate.AddHours(-24) || DateTime.Now >= targetReservation.EndDate)
            {
                throw new InvalidOperationException("Reservations cannot be canceled within 24 hours of the start date, during the stay, or after completion.");
            }
            var room = await _roomRepository.GetRoomByIdAsync(targetReservation.RoomId);
            room.IsAvailable = true;
            var hotel = await _hotelRepository.GetHotelAsync(room.HotelId);
            var city = await _cityRepository.GetCityByIdAsync(hotel.CityId);
            hotel.NumberOfVisits--;
            city.NumberOfVisits--;
            await _reservationRepository.DeleteReservationAsync(targetReservation);
        }
    }
}
