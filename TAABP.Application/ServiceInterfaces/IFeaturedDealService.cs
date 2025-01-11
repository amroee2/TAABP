using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IFeaturedDealService
    {
        Task<FeatueredDealDto> GetFeaturedDealByIdAsync(int id);
        Task<List<FeatueredDealDto>> GetFeaturedDealsAsync();
        Task<int> CreateFeaturedDealAsync(FeatueredDealDto featuredDealDto);
        Task UpdateFeaturedDealAsync(FeatueredDealDto featuredDealDto);
        Task DeleteFeaturedDealAsync(int id);
    }
}
