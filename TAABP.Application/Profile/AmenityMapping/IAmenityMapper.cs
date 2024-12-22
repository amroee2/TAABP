using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.AmenityMapping
{
    public partial interface IAmenityMapper
    {
        Amenity AmenityDtoToAmenity(AmenityDto amenityDto);
        AmenityDto AmenityToAmenityDto(Amenity amenity);
    }
}
