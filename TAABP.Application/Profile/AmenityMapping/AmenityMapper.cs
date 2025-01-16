using Riok.Mapperly.Abstractions;
using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.AmenityMapping
{
    [Mapper]
    public partial class AmenityMapper : IAmenityMapper
    {
        public partial void AmenityDtoToAmenity(AmenityDto amenityDto, Amenity amenity);
        public partial AmenityDto AmenityToAmenityDto(Amenity amenity);
    }
}
