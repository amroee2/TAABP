using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.AmenityMapping
{
    public partial interface IAmenityMapper
    {
        void AmenityDtoToAmenity(AmenityDto amenityDto, Amenity amenity);
        AmenityDto AmenityToAmenityDto(Amenity amenity);
    }
}
