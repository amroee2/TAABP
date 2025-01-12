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
    [Route("api/Users/{userId}/PayPals")]
    [ApiController]
    [Authorize]
    public class PayPalController : ControllerBase
    {
        private readonly IPayPalService _payPalService;
        private readonly ILogger _logger;
        private readonly IValidator<PayPalDto> _payPalValidator;
        public PayPalController(IPayPalService payPalService, IValidator<PayPalDto> validator)
        {
            _payPalService = payPalService;
            _logger = Log.ForContext<PayPalController>();
            _payPalValidator = validator;
        }

        [HttpGet("{paymentOptionId}")]
        public async Task<IActionResult> GetPaymentOptionByIdAsync(int paymentOptionId)
        {
            _logger.Information("Fetching payment option with ID {PaymentOptionId}", paymentOptionId);
            try
            {
                var paymentOption = await _payPalService.GetPaymentOptionByIdAsync(paymentOptionId);
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
        public async Task<IActionResult> AddNewPaymentOptionAsync(string userId, PayPalDto paymentOption)
        {
            _logger.Information("Adding a new payment option for user with ID {UserId}", userId);
            try
            {
                await _payPalValidator.ValidateAndThrowAsync(paymentOption);
                int payPalId = await _payPalService.AddNewPaymentOptionAsync(userId, paymentOption);
                var payPal = await _payPalService.GetPaymentOptionByIdAsync(payPalId);
                _logger.Information("Successfully added a new payment option with ID {PayPalId} for user with ID {UserId}", payPalId, userId);
                return StatusCode(201, payPal);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("User with ID {UserId} not found", userId);
                return NotFound(ex.Message);
            }
            catch(EmailAlreadyExistsException ex)
            {
                _logger.Warning("Email already exists");
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while adding a new payment option for user with ID {UserId}", userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{payPalId}")]
        public async Task<IActionResult> UpdatePaymentOptionAsync(int payPalId, string userId, PayPalDto paymentOption)
        {
            _logger.Information("Updating payment option with ID {PayPalId} for user with ID {UserId}", payPalId, userId);
            try
            {
                await _payPalValidator.ValidateAndThrowAsync(paymentOption);
                await _payPalService.UpdatePaymentOptionAsync(payPalId, userId, paymentOption);
                _logger.Information("Successfully updated payment option with ID {PayPalId} for user with ID {UserId}", payPalId, userId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Payment option with ID {PayPalId} for user with ID {UserId} not found", payPalId, userId);
                return NotFound(ex.Message);
            }
            catch(EmailAlreadyExistsException ex)
            {
                _logger.Warning("Email already exists");
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating payment option with ID {PayPalId} for user with ID {UserId}", payPalId, userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{payPalId}")]
        public async Task<IActionResult> DeletePaymentOptionAsync(string userId, int payPalId)
        {
            _logger.Information("Deleting payment option with ID {PayPalId} for user with ID {UserId}", payPalId, userId);
            try
            {
                await _payPalService.DeletePaymentOptionAsync(userId, payPalId);
                _logger.Information("Successfully deleted payment option with ID {PayPalId} for user with ID {UserId}", payPalId, userId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Payment option with ID {PayPalId} for user with ID {UserId} not found", payPalId, userId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while deleting payment option with ID {PayPalId} for user with ID {UserId}", payPalId, userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
