﻿using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.CityMapping;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;
        private readonly ICityMapper _cityMapper;

        public CityController(ICityService cityService, ICityMapper cityMapper)
        {
            _cityService = cityService;
            _cityMapper = cityMapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCitiesAsync()
        {
            var cities = await _cityService.GetCitiesAsync();
            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityByIdAsync(int id)
        {
            try
            {
                var city = await _cityService.GetCityByIdAsync(id);
                return Ok(city);

            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCityAsync(CityDto cityDto)
        {
            await _cityService.CreateCityAsync(cityDto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCityAsync(int id, CityDto cityDto)
        {
            cityDto.CityId = id;
            try
            {
                await _cityService.UpdateCityAsync(cityDto);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCityAsync(int id)
        {
            try
            {
                await _cityService.DeleteCityAsync(id);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            return NoContent();
        }
    }
}
