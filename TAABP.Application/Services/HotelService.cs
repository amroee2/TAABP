using Microsoft.AspNetCore.Identity;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.HotelMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core;

namespace TAABP.Application.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IHotelMapper _hotelMapper;
        public HotelService(IHotelRepository hotelRepository, IHotelMapper hotelMapper)
        {
            _hotelRepository = hotelRepository;
            _hotelMapper = hotelMapper;

        }

        public async Task CreateHotelAsync(HotelDto hotelDto)
        {
            var hotel = _hotelMapper.HotelDtoToUser(hotelDto);
            hotel.CreatedAt = DateTime.Now;
            hotel.CreatedBy = "System";
            await _hotelRepository.CreateHotelAsync(hotel);
        }

        public async Task<HotelDto> GetHotelAsync(int id)
        {
            var hotel = await _hotelRepository.GetHotelAsync(id);
            if (hotel == null)
            {
                throw new EntityNotFoundException($"Hotel with id {id} not found");
            }
            return _hotelMapper.HotelToHotelDto(hotel);
        }

        public async Task<List<HotelDto>> GetHotelsAsync()
        {
            var hotels = await _hotelRepository.GetHotelsAsync();
            return hotels.Select(hotel => _hotelMapper.HotelToHotelDto(hotel)).ToList();
        }

        public async Task DeleteHotelAsync(int id)
        {
            var hotel = await _hotelRepository.GetHotelAsync(id);
            if (hotel == null)
            {
                throw new EntityNotFoundException($"Hotel with id {id} not found");
            }
            await _hotelRepository.DeleteHotelAsync(hotel);
        }
    }
}
