using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IStripePaymentService _stripePaymentService;
        private readonly IPaymentService _paymentService;
        private readonly IDoctorService _doctorService;
        public PaymentsController(IStripePaymentService stripePaymentService ,
            IPaymentService paymentService ,
            IDoctorService doctorService
            )
        {
            _stripePaymentService = stripePaymentService;
            _paymentService = paymentService;
            _doctorService = doctorService;
        }

        [HttpPost("create-session")]
        [AllowAnonymous] 
        public async Task<IActionResult> CreatePaymentSession([FromBody] PaymentRequestDto dto)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(dto.DoctorId);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            var existingPayment = await _paymentService.GetByReferenceIdAsync(doctor.AppUserId);
            if (existingPayment != null && existingPayment.Status != PaymentStatus.Failed)
                return BadRequest(new { message = "Payment already exists for this doctor." });

            dto.ReferenceId = doctor.AppUserId;

            var result = await _stripePaymentService.CreateCheckoutSessionAsync(dto);

            if (result == null)
                return BadRequest(new { message = "Failed to create payment session." });

            return Ok(new
            {
                message = "Payment session created successfully.",
                sessionUrl = result.CheckoutUrl 
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
