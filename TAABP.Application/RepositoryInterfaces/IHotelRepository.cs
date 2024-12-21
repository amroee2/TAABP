using TAABP.Core;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IHotelRepository
    {
        Task CreateHotelAsync(Hotel hotel);
    }
}
