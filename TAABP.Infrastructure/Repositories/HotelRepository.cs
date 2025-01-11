using Microsoft.EntityFrameworkCore;
using TAABP.Application.DTOs;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core;

namespace TAABP.Infrastructure.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly TAABPDbContext _context;

        public HotelRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public async Task CreateHotelAsync(Hotel hotel)
        {
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task<Hotel> GetHotelByIdAsync(int id)
        {
            return await _context.Hotels.AsNoTracking().FirstOrDefaultAsync(h => h.HotelId == id);
        }

        public async Task<List<Hotel>> GetHotelsAsync()
        {
            return await _context.Hotels.AsNoTracking().ToListAsync();
        }

        public async Task DeleteHotelAsync(Hotel hotel)
        {
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateHotelAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task CreateNewHotelImageAsync(HotelImage hotelImage)
        {
            _context.HotelImages.Add(hotelImage);
            await _context.SaveChangesAsync();
        }

        public async Task<HotelImage> GetHotelImageByIdAsync(int hotelId, int imageId)
        {
            return await _context.HotelImages.AsNoTracking().FirstOrDefaultAsync(h => h.HotelId == hotelId && h.HotelImageId == imageId);
        }

        public async Task<List<HotelImage>> GetHotelImagesAsync(int hotelId)
        {
            return await _context.HotelImages.AsNoTracking().Where(h => h.HotelId == hotelId).ToListAsync();
        }

        public async Task DeleteHotelImageAsync(HotelImage hotelImage)
        {
            _context.HotelImages.Remove(hotelImage);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateHotelImageAsync(HotelImage hotelImage)
        {
            _context.HotelImages.Update(hotelImage);
            await _context.SaveChangesAsync();
        }

        public async Task IncrementNumberOfVisitsAsync(int hotelId)
        {
            var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == hotelId);
            hotel!.NumberOfVisits++;
            await _context.SaveChangesAsync();
        }

        public async Task DecrementNumberOfVisitsAsync(int hotelId)
        {
            var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == hotelId);
            hotel!.NumberOfVisits--;
            await _context.SaveChangesAsync();
        }

        public async Task IncrementNumberOfRoomsAsync(int hotelId)
        {
            var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == hotelId);
            hotel!.NumberOfRooms++;
            await _context.SaveChangesAsync();
        }

        public async Task DecrementNumberOfRoomsAsync(int hotelId)
        {
            var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.HotelId == hotelId);
            hotel!.NumberOfRooms--;
            await _context.SaveChangesAsync();
        }

        public async Task<List<Hotel>> GetFilteredHotelsAsync(FilterOptionsDto request)
        {
            var query = _context.Hotels
                .AsQueryable();

            if (request != null)
            {
                if (request.PriceRange != null && request.PriceRange.Length == 2)
                {
                    query = query.Where(h => h.Rooms.Any(r => r.PricePerNight >= request.PriceRange[0]
                                                              && r.PricePerNight <= request.PriceRange[1]));
                }

                if (request.StarRating != null && request.StarRating.Length > 0)
                {
                    query = query.Where(h => request.StarRating.Contains(h.Rating));
                }

                if (request.Amenities != null && request.Amenities.Length > 0)
                {
                    query = query.Where(h => h.Amenities.Any(a => request.Amenities.Contains(a.Name)));
                }

                if (!string.IsNullOrEmpty(request.RoomType) && Enum.TryParse(typeof(RoomType), request.RoomType, out var roomType))
                {
                    query = query.Where(h => h.Rooms.Any(r => r.Type == (RoomType)roomType));
                }
            }

            var totalResults = await query.CountAsync();

            var hotels = await query
                .OrderByDescending(h => h.Rating)
                .ToListAsync();

            return hotels;
        }
    }
}
