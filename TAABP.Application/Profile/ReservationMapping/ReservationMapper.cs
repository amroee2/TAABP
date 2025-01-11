using Riok.Mapperly.Abstractions;
using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.ReservationMapping
{
    [Mapper]
    public partial class ReservationMapper : IReservationMapper
    {
        public partial ReservationDto ReservationToResevationDto(Reservation entity);
        public partial Reservation ReservationDtoToReservation(ReservationDto dto);
    }
}
