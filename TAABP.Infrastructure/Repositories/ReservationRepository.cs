using Microsoft.EntityFrameworkCore;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Core;

namespace TAABP.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly TAABPDbContext _context;

        public ReservationRepository(TAABPDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reservation>> GetReservationsAsync(string userId)
        {
            return await _context.Reservations.AsNoTracking().Where(r => r.UserId == userId).ToListAsync();
        }

        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            return await _context.Reservations.AsNoTracking().FirstOrDefaultAsync(r => r.ReservationId == id);
        }

        public async Task CreateReservationAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReservationAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReservationAsync(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }
}
