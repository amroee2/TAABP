using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.RoomMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomMapper _roomMapper;
        private readonly IHotelRepository _hotelRepository;
        public RoomService(IRoomRepository roomRepository, IRoomMapper roomMapper, IHotelRepository hotelRepository)
        {
            _roomRepository = roomRepository;
            _roomMapper = roomMapper;
            _hotelRepository = hotelRepository;
        }

        public async Task<RoomDto> GetRoomAsync(int id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if(room == null)
            {
                throw new EntityNotFoundException("Room not found");
            }
            return _roomMapper.RoomToRoomDto(room);
        }

        public async Task<List<RoomDto>> GetRoomsAsync(int hotelId)
        {
            if (await _hotelRepository.GetHotelAsync(hotelId) == null)
            {
                throw new EntityNotFoundException("Hotel not found");
            }
            var rooms = await _roomRepository.GetRoomsAsync(hotelId);
            return rooms.Select(r => _roomMapper.RoomToRoomDto(r)).ToList();
        }

        public async Task CreateRoomAsync(RoomDto roomDto)
        {
            var hotel = await _hotelRepository.GetHotelAsync(roomDto.HotelId);
            if (hotel == null)
            {
                throw new EntityNotFoundException("Hotel not found");
            }

            var room = _roomMapper.RoomDtoToRoom(roomDto);
            room.CreatedAt = DateTime.Now;
            room.CreatedBy = "System";
            await _roomRepository.CreateRoomAsync(room);
        }

        public async Task UpdateRoomAsync(RoomDto roomDto)
        {
            var hotel = await _hotelRepository.GetHotelAsync(roomDto.HotelId);
            if (hotel == null)
            {
                throw new EntityNotFoundException("Hotel not found");
            }
            var room = await _roomRepository.GetRoomByIdAsync(roomDto.RoomId);
            if (room == null)
            {
                throw new EntityNotFoundException("Room not found");
            }
            room = _roomMapper.RoomDtoToRoom(roomDto);
            room.UpdatedAt = DateTime.Now;
            room.UpdatedBy = "System";
            await _roomRepository.UpdateRoomAsync(room);
        }

        public async Task DeleteRoomAsync(int id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if (room == null)
            {
                throw new EntityNotFoundException("Room not found");
            }
            await _roomRepository.DeleteRoomAsync(room);
        }

        public async Task CreateRoomImageAsync(RoomImageDto roomImageDto)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomImageDto.RoomId);
            if (room == null)
            {
                throw new EntityNotFoundException("Room not found");
            }
            var roomImage = _roomMapper.RoomImageDtoToRoomImage(roomImageDto);
            await _roomRepository.CreateRoomImageAsync(roomImage);
        }

        public async Task DeleteRoomImageAsync(int id)
        {
            var roomImage = await _roomRepository.GetRoomImageAsync(id);
            if (roomImage == null)
            {
                throw new EntityNotFoundException("Room image not found");
            }
            await _roomRepository.DeleteRoomImageAsync(roomImage);
        }

        public async Task<RoomImageDto> GetRoomImageAsync(int id)
        {
            var roomImage = await _roomRepository.GetRoomImageAsync(id);
            if (roomImage == null)
            {
                throw new EntityNotFoundException("Room image not found");
            }
            return _roomMapper.RoomImageToRoomImageDto(roomImage);
        }

        public async Task <List<RoomImageDto>> GetRoomImagesAsync(int roomId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null)
            {
                throw new EntityNotFoundException("Room not found");
            }
            var roomImages = await _roomRepository.GetRoomImagesAsync(roomId);
            return roomImages.Select(ri => _roomMapper.RoomImageToRoomImageDto(ri)).ToList();
        }
    }
}
