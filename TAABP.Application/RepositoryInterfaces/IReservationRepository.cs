﻿using TAABP.Core;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IReservationRepository
    {
        Task<List<Reservation>> GetReservationsAsync();
        Task<Reservation> GetReservationByIdAsync(int id);
        Task CreateReservationAsync(Reservation reservation);
        Task UpdateReservationAsync(Reservation reservation);
        Task DeleteReservationAsync(Reservation reservation);
    }
}
