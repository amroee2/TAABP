using Microsoft.EntityFrameworkCore;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core;

namespace TAABP.Infrastructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly TAABPDbContext _context;

        public RoomRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public async Task<Room> GetRoomByIdAsync(int id)
        {
            return await _context.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.RoomId == id);
        }

        public async Task<List<Room>> GetRoomsAsync()
        {
            return await _context.Rooms.AsNoTracking().ToListAsync();
        }

        public async Task CreateRoomAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoomAsync(Room room)
        {
            _context.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoomAsync(Room room)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
    }
}
