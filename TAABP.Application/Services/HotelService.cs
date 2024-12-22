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
            var hotel = _hotelMapper.HotelDtoToHotel(hotelDto);
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

        public async Task UpdateHotelAsync(int Id, HotelDto hotelDto)
        {
            var hotel = await _hotelRepository.GetHotelAsync(Id);
            if (hotel == null)
            {
                throw new EntityNotFoundException($"Hotel with id {Id} not found");
            }
            hotel = _hotelMapper.HotelDtoToHotel(hotelDto);
            hotel.HotelId = Id;
            hotel.UpdatedAt = DateTime.Now;
            hotel.UpdatedBy = "System";
            await _hotelRepository.UpdateHotelAsync(hotel);
        }

        public async Task AddNewImageAsync(int Id, string Image)
        {
            var hotel = await _hotelRepository.GetHotelAsync(Id);
            if (hotel == null)
            {
                throw new EntityNotFoundException($"Hotel with id {Id} not found");
            }
            HotelImage hotelImage = new HotelImage
            {
                HotelId = Id,
                ImageUrl = Image
            };
            await _hotelRepository.AddNewImageAsync(hotelImage);
        }

        public async Task<string> GetHotelImage(int hotelId, int imageId)
        {
            var hotelImage = await _hotelRepository.GetHotelImageAsync(hotelId, imageId);
            if (hotelImage == null)
            {
                throw new EntityNotFoundException($"Hotel Image with id {imageId} not found");
            }
            return hotelImage.ImageUrl;
        }

        public async Task<List<string>> GetHotelImages(int hotelId)
        {
            var hotelImages = await _hotelRepository.GetHotelImagesAsync(hotelId);
            return hotelImages.Select(hotelImage => hotelImage.ImageUrl).ToList();
        }

        public async Task DeleteHotelImageAsync(int hotelId, int imageId)
        {
            var hotelImage = await _hotelRepository.GetHotelImageAsync(hotelId, imageId);
            if (hotelImage == null)
            {
                throw new EntityNotFoundException($"Hotel Image with id {imageId} not found");
            }
            await _hotelRepository.DeleteHotelImageAsync(hotelImage);
        }
    }
}
