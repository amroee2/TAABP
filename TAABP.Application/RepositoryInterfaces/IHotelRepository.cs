using TAABP.Core;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IHotelRepository
    {
        Task CreateHotelAsync(Hotel hotel);
        Task<Hotel> GetHotelAsync(int id);
        Task<List<Hotel>> GetHotelsAsync();
        Task DeleteHotelAsync(Hotel hotel);
    }
}
