using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IReservationService
    {
        Task<ReservationDto> GetReservationByIdAsync(int id);
        Task<List<ReservationDto>> GetReservationsAsync();
        Task CreateReservationAsync(ReservationDto reservationDto);
        Task UpdateReservationAsync(ReservationDto reservationDto);
        Task DeleteReservationAsync(int id);
    }
}
