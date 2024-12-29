using TAABP.Core;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface ICityRepository
    {
        Task<List<City>> GetCitiesAsync();
        Task<City> GetCityByIdAsync(int id);
        Task CreateCityAsync(City city);
        Task UpdateCityAsync(City city);
        Task DeleteCityAsync(City city);
        Task IncrementNumberOfHotelsAsync(int cityId);
        Task DecrementNumberOfHotelsAsync(int cityId);
        Task IncrementNumberOfVisitsAsync(int cityId);
        Task DecrementNumberOfVisitsAsync(int cityId);
    }
}
