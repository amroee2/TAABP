using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.FeaturedDealMapping
{
    public partial interface IFeaturedDealMapper
    {
        FeaturedDeal FeaturedDealDtoToFeaturedDeal(FeatueredDealDto featuredDealDto);
        FeatueredDealDto FeaturedDealToFeaturedDealDto(FeaturedDeal featuredDeal);
    }
}
