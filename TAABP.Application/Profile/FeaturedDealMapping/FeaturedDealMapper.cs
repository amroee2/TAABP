using Riok.Mapperly.Abstractions;
using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.FeaturedDealMapping
{
    [Mapper]
    public partial class FeaturedDealMapper : IFeaturedDealMapper
    {
        public partial FeaturedDeal FeaturedDealDtoToFeaturedDeal(FeatueredDealDto featuredDealDto);
        public partial FeatueredDealDto FeaturedDealToFeaturedDealDto(FeaturedDeal featuredDeal);
    }
}
