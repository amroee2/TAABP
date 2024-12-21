﻿using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IHotelService
    {
        Task CreateHotelAsync( HotelDto hotelDto);
        Task<HotelDto> GetHotelAsync(int id);
        Task<List<HotelDto>> GetHotelsAsync();
        Task DeleteHotelAsync(int id);
    }
}
