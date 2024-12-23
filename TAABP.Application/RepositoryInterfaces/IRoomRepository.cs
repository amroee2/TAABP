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
    }
}
