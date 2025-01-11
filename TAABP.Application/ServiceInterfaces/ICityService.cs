using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface ICityService
    {
        Task<List<CityDto>> GetCitiesAsync();
        Task<CityDto> GetCityByIdAsync(int id);
        Task<int> CreateCityAsync(CityDto cityDto);
        Task UpdateCityAsync(CityDto cityDto);
        Task DeleteCityAsync(int id);
        Task<List<CityDto>> GetTopCitiesAsync();
    }
}
