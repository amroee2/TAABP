using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.ReservationMapping
{
    public interface IReservationMapper
    {
        ReservationDto ReservationToResevationDto(Reservation entity);
        Reservation ReservationDtoToReservation(ReservationDto dto);
    }
}
