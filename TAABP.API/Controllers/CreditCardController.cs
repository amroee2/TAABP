using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/{userId}/CreditCards")]
    [ApiController]
    public class CreditCardController : ControllerBase
    {
        private readonly ICreditCardService _creditCardService;

        public CreditCardController(ICreditCardService creditCardService)
        {
            _creditCardService = creditCardService;
        }

        [HttpGet("{paymentOptionId}")]
        public async Task<IActionResult> GetPaymentOptionByIdAsync(int paymentOptionId)
        {
            try
            {
                var paymentOption = await _creditCardService.GetPaymentOptionByIdAsync(paymentOptionId);
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
        public async Task<IActionResult> AddNewPaymentOptionAsync(string userId, CreditCardDto paymentOption)
        {
            try
            {
                int cardId = await _creditCardService.AddNewPaymentOptionAsync(userId, paymentOption);
                var card = await _creditCardService.GetPaymentOptionByIdAsync(cardId);
                return StatusCode(201, card);
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

        [HttpPut("{creditcardId}")]
        public async Task<IActionResult> UpdatePaymentOptionAsync(int creditcardId, string userId, CreditCardDto paymentOption)
        {
            try
            {
                await _creditCardService.UpdatePaymentOptionAsync(creditcardId, userId, paymentOption);
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

        [HttpDelete("{creditCardId}")]
        public async Task<IActionResult> DeletePaymentOptionAsync(string userId, int creditCardId)
        {
            try
            {
                await _creditCardService.DeletePaymentOptionAsync(userId, creditCardId);
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
