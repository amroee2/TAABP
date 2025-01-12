using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.ReservationMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core;

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

        public async Task<ReservationDto> GetReservationByIdAsync(string userId, int roomId, int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                throw new EntityNotFoundException("Reservation not found");
            }
            var room = await _roomRepository.GetRoomByIdAsync(reservation.RoomId);
            if(room == null)
            {
                throw new EntityNotFoundException("Room not found");
            }
            var user = await _userRepository.GetUserByIdAsync(reservation.UserId);
            if (user == null)
            {
                throw new EntityNotFoundException("User not found");
            }
            if(reservation.UserId != userId || reservation.RoomId != roomId)
            {
                throw new EntityNotFoundException("Reservation does not belong to user or room");
            }
            return _reservationMapper.ReservationToResevationDto(reservation);
        }

        public async Task<List<ReservationDto>> GetReservationsAsync()
        {
            var reservations = await _reservationRepository.GetReservationsAsync();
            return reservations.Select(r => _reservationMapper.ReservationToResevationDto(r)).ToList();
        }

        public async Task<int> CreateReservationAsync(ReservationDto reservationDto)
        {
            var room = await _roomRepository.GetRoomByIdAsync(reservationDto.RoomId);
            var user = await _userRepository.GetUserByIdAsync(reservationDto.UserId);
            if (room == null || user == null)
            {
                throw new EntityNotFoundException("Room or user not found");
            }
            if (!room.IsAvailable)
            {
                throw new InvalidOperationException("Room is not available");
            }
            var reservation = _reservationMapper.ReservationDtoToReservation(reservationDto);
            if (reservation.StartDate < DateTime.Now)
            {
                throw new InvalidOperationException("Start date cannot be in the past");
            }
            if (reservation.StartDate > reservation.EndDate)
            {
                throw new InvalidOperationException("Start date must be before end date");
            }
            var hotel = await _hotelRepository.GetHotelByIdAsync(room.HotelId);
            await _roomRepository.BookRoomAsync(room.RoomId);
            await _hotelRepository.IncrementNumberOfVisitsAsync(room.HotelId);
            await _cityRepository.IncrementNumberOfVisitsAsync(hotel.CityId);
            var price = (reservation.EndDate - reservation.StartDate).Days * room.PricePerNight;
            if(price == 0)
            {
                price = room.PricePerNight;
            }
            reservation.Price = price;
            await _reservationRepository.CreateReservationAsync(reservation);
            return reservation.ReservationId;
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
            if (reservationDto.StartDate < DateTime.Now)
            {
                throw new InvalidOperationException("Start date cannot be in the past");
            }
            var reservation = _reservationMapper.ReservationDtoToReservation(reservationDto);
            var room = await _roomRepository.GetRoomByIdAsync(reservation.RoomId);
            if(room == null)
            {
                throw new EntityNotFoundException("Room not found");
            }
            var price = (reservation.EndDate - reservation.StartDate).Days * room.PricePerNight;
            if (price == 0)
            {
                price = room.PricePerNight;
            }
            reservation.Price = price;
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
            await _roomRepository.UnbookRoomAsync(room.RoomId);
            var hotel = await _hotelRepository.GetHotelByIdAsync(room.HotelId);
            await _hotelRepository.DecrementNumberOfVisitsAsync(hotel.HotelId);
            await _cityRepository.DecrementNumberOfVisitsAsync(hotel.CityId);
            await _reservationRepository.DeleteReservationAsync(targetReservation);
        }
    }
}
