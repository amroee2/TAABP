using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.AmenityMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core;

namespace TAABP.Application.Services
{
    public class AmenityService : IAmenityService
    {
        private readonly IAmenityRepository _amenityRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IAmenityMapper _amenityMapper;
        public AmenityService(IAmenityMapper amenityMapper, IAmenityRepository amenityRepository, IHotelRepository hotelRepository)
        {
            _amenityRepository = amenityRepository;
            _hotelRepository = hotelRepository;
            _amenityMapper = amenityMapper;
        }

        public async Task<List<AmenityDto>> GetHotelAmenitiesAsync(int hotelId)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(hotelId);
            if (hotel == null)
            {
                throw new EntityNotFoundException("Hotel not found");
            }
            var amenities = await _amenityRepository.GetHotelAmenitiesAsync(hotelId);
            return amenities.Select(amenity => _amenityMapper.AmenityToAmenityDto(amenity)).ToList();
        }

        public async Task<AmenityDto> GetAmenityByIdAsync(int hotelId, int amenityId)
        {
            var amenity = await _amenityRepository.GetAmenityByIdAsync(amenityId);
            var hotel = await _hotelRepository.GetHotelByIdAsync(hotelId);
            if (amenity == null || hotel == null || hotel.HotelId!= amenity.HotelId)
            {
                throw new EntityNotFoundException("Amenity not found");
            }
            return _amenityMapper.AmenityToAmenityDto(amenity);
        }

        public async Task<int> CreateAmenityAsync(AmenityDto amenityDto)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(amenityDto.HotelId);
            if (hotel == null)
            {
                throw new EntityNotFoundException("Hotel not found");
            }
            var amenity = new Amenity();
            _amenityMapper.AmenityDtoToAmenity(amenityDto, amenity);
            await _amenityRepository.CreateAmenityAsync(amenity);
            return amenity.AmenityId;
        }

        public async Task UpdateAmenityAsync(AmenityDto amenity)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(amenity.HotelId);
            var targetAmenity = await _amenityRepository.GetAmenityByIdAsync(amenity.AmenityId);
            if (targetAmenity == null || hotel == null || hotel.HotelId != targetAmenity.HotelId)
            {
                throw new EntityNotFoundException("Amenity not found");
            }
            _amenityMapper.AmenityDtoToAmenity(amenity, targetAmenity);
            await _amenityRepository.UpdateAmenityAsync(targetAmenity);
        }

        public async Task DeleteAmenityAsync(int hotelId, int amenityId)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(hotelId);
            var targetAmenity = await _amenityRepository.GetAmenityByIdAsync(amenityId);
            if (targetAmenity == null || hotel == null || hotel.HotelId != targetAmenity.HotelId)
            {
                throw new EntityNotFoundException("Amenity not found");
            }
            await _amenityRepository.DeleteAmenityAsync(targetAmenity);
        }
    }
}
