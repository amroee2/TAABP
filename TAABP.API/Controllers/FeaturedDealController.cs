using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeaturedDealController : ControllerBase
    {
        private readonly IFeaturedDealService _featuredDealService;

        public FeaturedDealController(IFeaturedDealService featuredDealService)
        {
            _featuredDealService = featuredDealService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFeaturedDealsAsync()
        {
            try
            {
                var featuredDeals = await _featuredDealService.GetFeaturedDealsAsync();
                return Ok(featuredDeals);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeaturedDealByIdAsync(int id)
        {
            try
            {
                var featuredDeal = await _featuredDealService.GetFeaturedDealByIdAsync(id);
                return Ok(featuredDeal);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeaturedDealAsync(FeatueredDealDto featuredDealDto)
        {
            try
            {
                await _featuredDealService.CreateFeaturedDealAsync(featuredDealDto);
                return Ok(new { message = "Featured deal created successfully!" });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeaturedDealAsync(int id, FeatueredDealDto featuredDealDto)
        {
            try
            {
                featuredDealDto.FeaturedDealId = id;
                await _featuredDealService.UpdateFeaturedDealAsync(featuredDealDto);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeaturedDealAsync(int id)
        {
            try
            {
                await _featuredDealService.DeleteFeaturedDealAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
