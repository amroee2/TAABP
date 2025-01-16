using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;
using ILogger = Serilog.ILogger;

namespace TAABP.API.Controllers
{
    [Route("api/Users/{userId}/Payments")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        public PaymentController(IPaymentMethodService paymentMethodService, IUserService userService)
        {
            _paymentMethodService = paymentMethodService;
            _logger = Log.ForContext<PaymentController>();
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPaymentOptions(string userId)
        {
            _logger.Information("Fetching payment options for user with ID {UserId}", userId);
            try
            {
                if (userId != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to payment options for user with ID {UserId}", userId);
                    return Unauthorized();
                }
                var paymentOptions = await _paymentMethodService.GetAllUserPaymentOptionsAsync(userId);
                _logger.Information("Successfully fetched payment options for user with ID {UserId}", userId);
                return Ok(paymentOptions);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("User with ID {UserId} not found", userId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching payment options for user with ID {UserId}", userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
