using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/{userId}/PayPals")]
    [ApiController]
    [Authorize]
    public class PayPalController : ControllerBase
    {
        private readonly IPayPalService _payPalService;

        public PayPalController(IPayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        [HttpGet("{paymentOptionId}")]
        public async Task<IActionResult> GetPaymentOptionByIdAsync(int paymentOptionId)
        {
            try
            {
                var paymentOption = await _payPalService.GetPaymentOptionByIdAsync(paymentOptionId);
                return Ok(paymentOption);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewPaymentOptionAsync(string userId, PayPalDto paymentOption)
        {
            try
            {
                int payPalId = await _payPalService.AddNewPaymentOptionAsync(userId, paymentOption);
                var payPal = await _payPalService.GetPaymentOptionByIdAsync(payPalId);
                return StatusCode(201, payPal);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(EmailAlreadyExistsException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{payPalId}")]
        public async Task<IActionResult> UpdatePaymentOptionAsync(int payPalId, string userId, PayPalDto paymentOption)
        {
            try
            {
                await _payPalService.UpdatePaymentOptionAsync(payPalId, userId, paymentOption);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(EmailAlreadyExistsException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{payPalId}")]
        public async Task<IActionResult> DeletePaymentOptionAsync(string userId, int payPalId)
        {
            try
            {
                await _payPalService.DeletePaymentOptionAsync(userId, payPalId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
