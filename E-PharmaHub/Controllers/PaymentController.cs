using E_PharmaHub.Dtos;
using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_PharmaHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IStripePaymentService _stripePaymentService;
        private readonly IPaymentService _paymentService;

        public PaymentsController(IStripePaymentService stripePaymentService , IPaymentService paymentService)
        {
            _stripePaymentService = stripePaymentService;
            _paymentService = paymentService;
        }

        [HttpPost("create-session")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> CreatePaymentSession([FromBody] PaymentRequestDto dto)
        {
            var result = await _stripePaymentService.CreateCheckoutSessionAsync(dto);
            return Ok(result);
        }
        [HttpGet("verify-session")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> VerifySession(string sessionId)
        {
            var result = await _paymentService.VerifySessionAsync(sessionId);
            return Ok(result);
        }



    }

}
