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

        public async Task<List<Room>> GetRoomsAsync(int hotelId)
        {
            return await _context.Rooms.AsNoTracking().Where(r => r.HotelId == hotelId).ToListAsync();
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

        public async Task<RoomImage> GetRoomImageAsync(int roomImageId)
        {
            return await _context.RoomImages.AsNoTracking().FirstOrDefaultAsync(ri => ri.RoomImageId == roomImageId);
        }

        public async Task<List<RoomImage>> GetRoomImagesAsync(int roomId)
        {
            return await _context.RoomImages.AsNoTracking().Where(ri => ri.RoomId == roomId).ToListAsync();
        }

        public async Task CreateRoomImageAsync(RoomImage roomImage)
        {
            _context.RoomImages.Add(roomImage);
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteRoomImageAsync(RoomImage roomImage)
        {
            _context.RoomImages.Remove(roomImage);
            await _context.SaveChangesAsync();
        }
    }
}
