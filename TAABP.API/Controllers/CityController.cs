using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.CityMapping;
using TAABP.Application.ServiceInterfaces;
using ILogger = Serilog.ILogger;

namespace TAABP.API.Controllers
{
    [Route("api/Cities")]
    [ApiController]
    [Authorize]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;
        private readonly ICityMapper _cityMapper;
        private readonly ILogger _logger;
        private readonly IValidator<CityDto> _cityValidator;
        public CityController(ICityService cityService, ICityMapper cityMapper, IValidator<CityDto> validator)
        {
            _cityService = cityService;
            _cityMapper = cityMapper;
            _logger = Log.ForContext<CityController>();
            _cityValidator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCitiesAsync()
        {
            var cities = await _cityService.GetCitiesAsync();
            _logger.Information("Fetched {CityCount} cities", cities.Count);
            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityByIdAsync(int id)
        {
            _logger.Information("Fetching city with ID {CityId}", id);
            try
            {
                var city = await _cityService.GetCityByIdAsync(id);
                _logger.Information("Successfully fetched city with ID {CityId}", id);
                return Ok(city);

            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("City with ID {CityId} not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching city with ID {CityId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCityAsync(CityDto cityDto)
        {
            _logger.Information("Creating city with name {CityName}", cityDto.Name);
            try
            {
                await _cityValidator.ValidateAndThrowAsync(cityDto);
                int id = await _cityService.CreateCityAsync(cityDto);
                var createdCity = await _cityService.GetCityByIdAsync(id);
                _logger.Information("Successfully created city with ID {CityId}", id);
                return StatusCode(201, createdCity);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("City with name {CityName} not found", cityDto.Name);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while creating city with name {CityName}", cityDto.Name);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCityAsync(int id, CityDto cityDto)
        {
            _logger.Information("Updating city with ID {CityId}", id);
            await _cityValidator.ValidateAndThrowAsync(cityDto);
            cityDto.CityId = id;
            try
            {
                await _cityService.UpdateCityAsync(cityDto);
                _logger.Information("Successfully updated city with ID {CityId}", id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("City with ID {CityId} not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating city with ID {CityId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCityAsync(int id)
        {
            _logger.Information("Deleting city with ID {CityId}", id);
            try
            {
                await _cityService.DeleteCityAsync(id);
                _logger.Information("Successfully deleted city with ID {CityId}", id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("City with ID {CityId} not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while deleting city with ID {CityId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("TopCities")]
        public async Task<IActionResult> GetTopCitiesAsync()
        {
            _logger.Information("Fetching top cities");
            try
            {
                var topCities = await _cityService.GetTopCitiesAsync();
                _logger.Information("Successfully fetched top cities");
                return Ok(topCities);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching top cities");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
