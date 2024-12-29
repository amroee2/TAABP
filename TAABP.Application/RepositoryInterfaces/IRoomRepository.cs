using TAABP.Core;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IRoomRepository
    {
        Task<Room> GetRoomByIdAsync(int id);
        Task<List<Room>> GetRoomsAsync(int hotelId);
        Task CreateRoomAsync(Room room);
        Task UpdateRoomAsync(Room room);
        Task DeleteRoomAsync(Room room);
        Task<RoomImage> GetRoomImageByIdAsync(int roomImageId);
        Task<List<RoomImage>> GetRoomImagesAsync(int roomId);
        Task CreateRoomImageAsync(RoomImage roomImage);
        Task DeleteRoomImageAsync(RoomImage roomImage);
        Task BookRoomAsync(int roomId);
        Task UnbookRoomAsync(int roomId);
    }
}
