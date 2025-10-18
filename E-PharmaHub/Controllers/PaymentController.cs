using E_PharmaHub.Dtos;
using E_PharmaHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_PharmaHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IStripePaymentService _stripePaymentService;
        private readonly IPaymentService _paymentService;
        private readonly IDoctorService _doctorService;
        private readonly IPharmacistService _pharmacistService;
        public PaymentsController(IStripePaymentService stripePaymentService ,
            IPaymentService paymentService ,
            IDoctorService doctorService ,
            IPharmacistService pharmacistService
            )
        {
            _stripePaymentService = stripePaymentService;
            _paymentService = paymentService;
            _doctorService = doctorService;
            _pharmacistService = pharmacistService;
        }

        [HttpPost("create-session")]
        public async Task<IActionResult> CreatePaymentSession([FromBody] PaymentRequestDto dto)
        {
            if (dto.DoctorId.HasValue)
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(dto.DoctorId.Value);
                if (doctor == null)
                    return NotFound(new { message = "Doctor not found." });

                dto.ReferenceId = doctor.AppUserId;
                dto.Amount = doctor.ConsultationPrice;
            }
            if (dto.PharmacistId.HasValue)
            {
                var doctor = await _pharmacistService.GetPharmacistProfileByIdAsync(dto.PharmacistId.Value);
                if (doctor == null)
                    return NotFound(new { message = "Doctor not found." });

                dto.ReferenceId = doctor.AppUserId;
            }

            var result = await _stripePaymentService.CreateCheckoutSessionAsync(dto);

            if (result == null)
                return BadRequest(new { message = "Failed to create payment session." });

            return Ok(new
            {
                message = "Payment session created successfully.",
                sessionUrl = result.CheckoutUrl,
                sessionId = result.SessionId,
            });
        }


        [HttpGet("verify-session")]

        public async Task<IActionResult> VerifySession(string sessionId)
        {
            var result = await _paymentService.VerifySessionAsync(sessionId);
            return Ok(result);
        }



    }

}
