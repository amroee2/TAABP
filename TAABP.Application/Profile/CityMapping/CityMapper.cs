﻿using Riok.Mapperly.Abstractions;
using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.CityMapping
{
    [Mapper]
    public partial class CityMapper : ICityMapper
    {
        public partial void CityDtoToCity(CityDto cityDto, City city);
        public partial CityDto CityToCityDto(City city);
    }
}
