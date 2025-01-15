using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TAABP.Application.DTOs;
using TAABP.Application.ServiceInterfaces;
using ILogger = Serilog.ILogger;

namespace TAABP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly ILogger _logger;
        private readonly IValidator<FilterOptionsDto> _filterOptionsValidator;
        public SearchController(IHotelService hotelService, IValidator<FilterOptionsDto> validator)
        {
            _hotelService = hotelService;
            _logger = Log.ForContext<SearchController>();
            _filterOptionsValidator = validator;
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetFilteredHotelsAsync([FromBody] FilterOptionsDto hotelFilter)
        {
            try
            {
                _logger.Information("Fetching hotels based on filter options");
                await _filterOptionsValidator.ValidateAndThrowAsync(hotelFilter);
                var hotels = await _hotelService.GetFilteredHotelsAsync(hotelFilter);
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching hotels based on filter options");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
