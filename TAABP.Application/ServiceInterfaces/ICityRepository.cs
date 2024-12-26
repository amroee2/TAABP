using TAABP.Core;

namespace TAABP.Application.ServiceInterfaces
{
    public interface ICityRepository
    {
        Task<List<City>> GetCitiesAsync();
        Task<City> GetCityByIdAsync(int id);
        Task CreateCityAsync(City city);
        Task UpdateCityAsync(City city);
        Task DeleteCityAsync(City city);
    }
}
