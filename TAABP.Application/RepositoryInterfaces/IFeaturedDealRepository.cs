using TAABP.Core;

namespace TAABP.Application.RepositoryInterfaces
{
    public interface IFeaturedDealRepository
    {
        Task<FeaturedDeal> GetFeaturedDealAsync(int id);
        Task<List<FeaturedDeal>> GetFeaturedDealsAsync();
        Task CreateFeaturedDealAsync(FeaturedDeal featuredDeal);
        Task UpdateFeaturedDealAsync(FeaturedDeal featuredDeal);
        Task DeleteFeaturedDealAsync(FeaturedDeal featuredDeal);
    }
}
