using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService;

        public PaymentController(IPaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPaymentOptions(string userId)
        {
            var paymentOptions = await _paymentMethodService.GetAllUserPaymentOptionsAsync(userId);
            return Ok(paymentOptions);
        }
    }
}
