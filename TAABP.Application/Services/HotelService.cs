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
        private readonly IUserService _userService;
        private readonly ICityRepository _cityRepository;
        public HotelService(ICityRepository cityRepository, IHotelRepository hotelRepository, IHotelMapper hotelMapper, IUserService userService)
        {
            _hotelRepository = hotelRepository;
            _hotelMapper = hotelMapper;
            _userService = userService;
            _cityRepository = cityRepository;

        }

        public async Task<int> CreateHotelAsync(int cityId, HotelDto hotelDto)
        {
            var city = await _cityRepository.GetCityByIdAsync(cityId);
            if (city == null)
            {
                throw new EntityNotFoundException($"City with id {cityId} not found");
            }
            var hotel = new Hotel();
            _hotelMapper.HotelDtoToHotel(hotelDto, hotel);
            hotel.CreatedAt = DateTime.Now;
            hotel.CreatedBy = await _userService.GetCurrentUsernameAsync();
            hotel.CityId = cityId;
            await _hotelRepository.CreateHotelAsync(hotel);
            await _cityRepository.IncrementNumberOfHotelsAsync(cityId);
            return hotel.HotelId;
        }

        public async Task<HotelDto> GetHotelByIdAsync(int cityId, int id)
        {
            var city = await _cityRepository.GetCityByIdAsync(cityId);
            if (city == null)
            {
                throw new EntityNotFoundException($"City with id {cityId} not found");
            }
            var hotel = await _hotelRepository.GetHotelByIdAsync(id);
            if (hotel == null)
            {
                throw new EntityNotFoundException($"Hotel with id {id} not found");
            }
            if(hotel.CityId != cityId)
            {
                throw new EntityNotFoundException($"Hotel with id {id} not found in city with id {cityId}");
            }
            return _hotelMapper.HotelToHotelDto(hotel);
        }

        public async Task<List<HotelDto>> GetHotelsAsync(int cityId)
        {
            var hotels = await _hotelRepository.GetHotelsAsync(cityId);
            return hotels.Select(hotel => _hotelMapper.HotelToHotelDto(hotel)).ToList();
        }

        public async Task DeleteHotelAsync(int cityId, int id)
        {
            var city = await _cityRepository.GetCityByIdAsync(cityId);
            if (city == null)
            {
                throw new EntityNotFoundException($"City with id {cityId} not found");
            }
            var hotel = await _hotelRepository.GetHotelByIdAsync(id);
            if (hotel == null)
            {
                throw new EntityNotFoundException($"Hotel with id {id} not found");
            }
            if (hotel.CityId != cityId)
            {
                throw new EntityNotFoundException($"Hotel with id {id} not found in city with id {cityId}");
            }
            await _hotelRepository.DeleteHotelAsync(hotel);
            await _cityRepository.DecrementNumberOfHotelsAsync(city.CityId);
        }

        public async Task UpdateHotelAsync(int cityId, HotelDto hotelDto)
        {
            var city = await _cityRepository.GetCityByIdAsync(cityId);
            if (city == null)
            {
                throw new EntityNotFoundException($"City with id {cityId} not found");
            }
            var targetHotel = await _hotelRepository.GetHotelByIdAsync(hotelDto.HotelId);
            if (targetHotel == null)
            {
                throw new EntityNotFoundException($"Hotel with id {hotelDto.HotelId} not found");
            }
            if (targetHotel.CityId != cityId)
            {
                throw new EntityNotFoundException($"Hotel with id {hotelDto.HotelId} not found in city with id {cityId}");
            }
            _hotelMapper.HotelDtoToHotel(hotelDto, targetHotel);
            targetHotel.UpdatedAt = DateTime.Now;
            targetHotel.UpdatedBy = await _userService.GetCurrentUsernameAsync();
            targetHotel.CityId = cityId;
            await _hotelRepository.UpdateHotelAsync(targetHotel);
        }

        public async Task<int> CreateNewHotelImageAsync(int Id, HotelImageDto hotelImageDto)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(Id);
            if (hotel == null)
            {
                throw new EntityNotFoundException($"Hotel with id {Id} not found");
            }
            var hotelImage = new HotelImage();
            _hotelMapper.HotelImageDtoToHotelImage(hotelImageDto, hotelImage);
            hotelImage.HotelId = Id;
            await _hotelRepository.CreateNewHotelImageAsync(hotelImage);
            return hotelImage.HotelImageId;
        }

        public async Task<HotelImageDto> GetHotelImageByIdAsync(int hotelId, int imageId)
        {
            var hotelImage = await _hotelRepository.GetHotelImageByIdAsync(hotelId, imageId);
            if (hotelImage == null)
            {
                throw new EntityNotFoundException($"Hotel Image with id {imageId} for hotel with id {hotelId} not found");
            }
            return _hotelMapper.HotelImageToHotelImageDto(hotelImage);
        }

        public async Task<List<HotelImageDto>> GetHotelImagesAsync(int hotelId)
        {
            var hotelImages = await _hotelRepository.GetHotelImagesAsync(hotelId);
            return hotelImages.Select(hotelImage => _hotelMapper.HotelImageToHotelImageDto(hotelImage)).ToList();
        }

        public async Task DeleteHotelImageAsync(int hotelId, int imageId)
        {
            var hotelImage = await _hotelRepository.GetHotelImageByIdAsync(hotelId, imageId);
            if (hotelImage == null)
            {
                throw new EntityNotFoundException($"Hotel Image with id {imageId} for hotel with id {hotelId} not found");
            }
            await _hotelRepository.DeleteHotelImageAsync(hotelImage);
        }

        public async Task UpdateHotelImageAsync(int hotelId, int imageId, HotelImageDto imageUrl)
        {
            var hotelImage = await _hotelRepository.GetHotelImageByIdAsync(hotelId, imageId);
            if (hotelImage == null)
            {
                throw new EntityNotFoundException($"Hotel Image with id {imageId} for hotel with id {hotelId} not found");
            }
            _hotelMapper.HotelImageDtoToHotelImage(imageUrl, hotelImage);
            await _hotelRepository.UpdateHotelImageAsync(hotelImage);
        }

        public async Task<HotelSearchResultDto> GetFilteredHotelsAsync(FilterOptionsDto hotelFilter)
        {
            var hotels = await _hotelRepository.GetFilteredHotelsAsync(hotelFilter);
            return new HotelSearchResultDto
            {
                Hotels = hotels.Select(hotel => _hotelMapper.HotelToHotelDto(hotel)).ToList(),
                TotalResults = hotels.Count
            };
        }
    }
}
