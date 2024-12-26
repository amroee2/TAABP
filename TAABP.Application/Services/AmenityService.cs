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

        public async Task<AmenityDto> GetAmenityAsync(int amenityId)
        {
            var amenity = await _amenityRepository.GetAmenityAsync(amenityId);
            if (amenity == null)
            {
                throw new EntityNotFoundException("Amenity not found");
            }
            return _amenityMapper.AmenityToAmenityDto(amenity);
        }

        public async Task CreateAmenityAsync(AmenityDto amenity)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(amenity.HotelId);
            if (hotel == null)
            {
                throw new EntityNotFoundException("Hotel not found");
            }
            await _amenityRepository.CreateAmenityAsync(_amenityMapper.AmenityDtoToAmenity(amenity));
        }

        public async Task UpdateAmenityAsync(AmenityDto amenity)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(amenity.HotelId);
            var targetAmenity = await _amenityRepository.GetAmenityAsync(amenity.AmenityId);
            if (hotel == null)
            {
                throw new EntityNotFoundException("Hotel not found");
            }
            if (targetAmenity == null)
            {
                throw new EntityNotFoundException("Amenity not found");
            }
            await _amenityRepository.UpdateAmenityAsync(_amenityMapper.AmenityDtoToAmenity(amenity));
        }

        public async Task DeleteAmenityAsync(AmenityDto amenity)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(amenity.HotelId);
            var targetAmenity = await _amenityRepository.GetAmenityAsync(amenity.AmenityId);
            if (hotel == null)
            {
                throw new EntityNotFoundException("Hotel not found");
            }
            if (targetAmenity == null)
            {
                throw new EntityNotFoundException("Amenity not found");
            }
            await _amenityRepository.DeleteAmenityAsync(_amenityMapper.AmenityDtoToAmenity(amenity));
        }
    }
}
