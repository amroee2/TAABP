using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.CityMapping
{
    public interface ICityMapper
    {
        public City CityDtoToCity(CityDto cityDto);
        CityDto CityToCityDto(City city);
    }
}
