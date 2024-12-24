using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.CityMapping;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.Application.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly ICityMapper _cityMapper;

        public CityService(ICityRepository cityRepository, ICityMapper cityMapper)
        {
            _cityRepository = cityRepository;
            _cityMapper = cityMapper;
        }

        public async Task<List<CityDto>> GetCitiesAsync()
        {
            var cities = await _cityRepository.GetCitiesAsync();
            return cities.Select(city => _cityMapper.CityToCityDto(city)).ToList();
        }

        public async Task<CityDto> GetCityByIdAsync(int id)
        {
            var city = await _cityRepository.GetCityByIdAsync(id);
            if(city == null)
            {
                throw new EntityNotFoundException("City not found");
            }
            return _cityMapper.CityToCityDto(city);
        }

        public async Task CreateCityAsync(CityDto cityDto)
        {
            var city = _cityMapper.CityDtoToCity(cityDto);
            city.CreatedAt = DateTime.Now;
            city.CreatedBy = "Admin";
            await _cityRepository.CreateCityAsync(city);
        }

        public async Task UpdateCityAsync(CityDto cityDto)
        {
            var targetCity = await _cityRepository.GetCityByIdAsync(cityDto.CityId);
            if (targetCity == null)
            {
                throw new EntityNotFoundException("City not found");
            }
            var city = _cityMapper.CityDtoToCity(cityDto);
            city.UpdatedAt = DateTime.Now;
            city.UpdatedBy = "Admin";
            await _cityRepository.UpdateCityAsync(city);
        }

        public async Task DeleteCityAsync(int id)
        {
            var city = await _cityRepository.GetCityByIdAsync(id);
            if (city == null)
            {
                throw new EntityNotFoundException("City not found");
            }
            await _cityRepository.DeleteCityAsync(city);
        }
    }
}
