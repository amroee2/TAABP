using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.CityMapping
{
    public interface ICityMapper
    {
        void CityDtoToCity(CityDto cityDto, City city);
        CityDto CityToCityDto(City city);
    }
}
