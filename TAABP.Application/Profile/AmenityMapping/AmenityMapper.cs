using Riok.Mapperly.Abstractions;
using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.AmenityMapping
{
    [Mapper]
    public partial class AmenityMapper : IAmenityMapper
    {
        public partial Amenity AmenityDtoToAmenity(AmenityDto amenityDto);
        public partial AmenityDto AmenityToAmenityDto(Amenity amenity);
    }
}
