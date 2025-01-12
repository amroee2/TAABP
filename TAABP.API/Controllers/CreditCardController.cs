using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;
using ILogger = Serilog.ILogger;

namespace TAABP.API.Controllers
{
    [Route("api/Users/{userId}/CreditCards")]
    [ApiController]
    [Authorize]
    public class CreditCardController : ControllerBase
    {
        private readonly ICreditCardService _creditCardService;
        private readonly ILogger _logger;
        private readonly IValidator<CreditCardDto> _creditCardValidator;
        private readonly IUserService _userService;
        public CreditCardController(ICreditCardService creditCardService,
            IValidator<CreditCardDto> validator,
            IUserService userService)
        {
            _creditCardService = creditCardService;
            _logger = Log.ForContext<CreditCardController>();
            _creditCardValidator = validator;
            _userService = userService;
        }

        [HttpGet("{paymentOptionId}")]
        public async Task<IActionResult> GetPaymentOptionByIdAsync(string userId, int paymentOptionId)
        {
            _logger.Information("Fetching payment option with ID {PaymentOptionId}", paymentOptionId);
            try
            {
                if(userId != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to payment option with ID {PaymentOptionId}", paymentOptionId);
                    return Unauthorized();
                }
                var paymentOption = await _creditCardService.GetPaymentOptionByIdAsync(paymentOptionId);
                _logger.Information("Successfully fetched payment option with ID {PaymentOptionId}", paymentOptionId);
                return Ok(paymentOption);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Payment option with ID {PaymentOptionId} not found", paymentOptionId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching payment option with ID {PaymentOptionId}", paymentOptionId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewPaymentOptionAsync(string userId, CreditCardDto paymentOption)
        {
            _logger.Information("Adding new payment option for user with ID {UserId}", userId);
            try
            {
                if (userId != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to add payment option for user with ID {UserId}", userId);
                    return Unauthorized();
                }
                await _creditCardValidator.ValidateAndThrowAsync(paymentOption);
                int cardId = await _creditCardService.AddNewPaymentOptionAsync(userId, paymentOption);
                var card = await _creditCardService.GetPaymentOptionByIdAsync(cardId);
                _logger.Information("Successfully added payment option with ID {PaymentOptionId} for user with ID {UserId}", cardId, userId);
                return StatusCode(201, card);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("User with ID {UserId} not found", userId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while adding payment option for user with ID {UserId}", userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{creditcardId}")]
        public async Task<IActionResult> UpdatePaymentOptionAsync(int creditcardId, string userId, CreditCardDto paymentOption)
        {
            _logger.Information("Updating payment option with ID {PaymentOptionId} for user with ID {UserId}", creditcardId, userId);
            try
            {
                if (userId != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to update payment option with ID {PaymentOptionId} for user with ID {UserId}", creditcardId, userId);
                    return Unauthorized();
                }
                await _creditCardValidator.ValidateAndThrowAsync(paymentOption);
                await _creditCardService.UpdatePaymentOptionAsync(creditcardId, userId, paymentOption);
                _logger.Information("Successfully updated payment option with ID {PaymentOptionId} for user with ID {UserId}", creditcardId, userId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Payment option with ID {PaymentOptionId} not found for user with ID {UserId}", creditcardId, userId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating payment option with ID {PaymentOptionId} for user with ID {UserId}", creditcardId, userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{creditCardId}")]
        public async Task<IActionResult> DeletePaymentOptionAsync(string userId, int creditCardId)
        {
            _logger.Information("Deleting payment option with ID {PaymentOptionId} for user with ID {UserId}", creditCardId, userId);
            try
            {
                if (userId != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to delete payment option with ID {PaymentOptionId} for user with ID {UserId}", creditCardId, userId);
                    return Unauthorized();
                }
                await _creditCardService.DeletePaymentOptionAsync(userId, creditCardId);
                _logger.Information("Successfully deleted payment option with ID {PaymentOptionId} for user with ID {UserId}", creditCardId, userId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Payment option with ID {PaymentOptionId} not found for user with ID {UserId}", creditCardId, userId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while deleting payment option with ID {PaymentOptionId} for user with ID {UserId}", creditCardId, userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
