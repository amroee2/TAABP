using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IReservationService
    {
        Task<ReservationDto> GetReservationByIdAsync(string userId, int roomId, int id);
        Task<List<ReservationDto>> GetReservationsAsync();
        Task<int> CreateReservationAsync(ReservationDto reservationDto);
        Task UpdateReservationAsync(ReservationDto reservationDto);
        Task DeleteReservationAsync(int id);
    }
}
