using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IRoomService
    {
        Task CreateRoomAsync(RoomDto roomDto);
        Task<RoomDto> GetRoomAsync(int id);
        Task<List<RoomDto>> GetRoomsAsync(int hotelId);
        Task DeleteRoomAsync(int id);
        Task UpdateRoomAsync(RoomDto roomDto);
        Task CreateRoomImageAsync(RoomImageDto roomImageDto);
        Task<RoomImageDto> GetRoomImageAsync(int id);
        Task<List<RoomImageDto>> GetRoomImagesAsync(int roomId);
        Task DeleteRoomImageAsync(int id);
    }
}
