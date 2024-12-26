using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.CityMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core;

namespace TAABP.Application.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly ICityMapper _cityMapper;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CityService(
            ICityRepository cityRepository,
            ICityMapper cityMapper,
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _cityRepository = cityRepository;
            _cityMapper = cityMapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<string> GetCurrentUsernameAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            return user.UserName;
        }

        public async Task<List<CityDto>> GetCitiesAsync()
        {
            var cities = await _cityRepository.GetCitiesAsync();
            return cities.Select(city => _cityMapper.CityToCityDto(city)).ToList();
        }

        public async Task<CityDto> GetCityByIdAsync(int id)
        {
            var city = await _cityRepository.GetCityByIdAsync(id);
            if (city == null)
            {
                throw new EntityNotFoundException("City not found");
            }
            return _cityMapper.CityToCityDto(city);
        }

        public async Task<int> CreateCityAsync(CityDto cityDto)
        {
            var city = new City();
            _cityMapper.CityDtoToCity(cityDto, city);
            city.CreatedAt = DateTime.Now;
            city.CreatedBy = await GetCurrentUsernameAsync();

            await _cityRepository.CreateCityAsync(city);

            return city.CityId;
        }


        public async Task UpdateCityAsync(CityDto cityDto)
        {
            var targetCity = await _cityRepository.GetCityByIdAsync(cityDto.CityId);
            if (targetCity == null)
            {
                throw new EntityNotFoundException("City not found");
            }
            _cityMapper.CityDtoToCity(cityDto, targetCity);

            targetCity.UpdatedAt = DateTime.Now;
            targetCity.UpdatedBy = await GetCurrentUsernameAsync();

            await _cityRepository.UpdateCityAsync(targetCity);
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
