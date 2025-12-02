using E_PharmaHub.Dtos;
using E_PharmaHub.Models.Enums;
using E_PharmaHub.Services.AppointmentServ;
using E_PharmaHub.Services.DoctorServ;
using E_PharmaHub.Services.OrderServ;
using E_PharmaHub.Services.PaymentServ;
using E_PharmaHub.Services.PharmacistServ;
using E_PharmaHub.Services.StripePaymentServ;
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
        private readonly IPharmacistService _pharmacistService;
        private readonly IAppointmentService _appointmentService;
        private readonly IOrderService _orderService;
        public PaymentsController(IStripePaymentService stripePaymentService ,
            IPaymentService paymentService ,
            IDoctorService doctorService ,
            IPharmacistService pharmacistService,
            IAppointmentService appointmentService,
            IOrderService orderService
            )
        {
            _stripePaymentService = stripePaymentService;
            _paymentService = paymentService;
            _doctorService = doctorService;
            _pharmacistService = pharmacistService;
            _appointmentService = appointmentService;
            _orderService = orderService;
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
            }
            if (dto.PharmacistId.HasValue)
            {
                var doctor = await _pharmacistService.GetPharmacistProfileByIdAsync(dto.PharmacistId.Value);
                if (doctor == null)
                    return NotFound(new { message = "Doctor not found." });

                dto.ReferenceId = doctor.AppUserId;
            }
            if(dto.PaymentFor == PaymentForType.Appointment && dto.AppointmentId.HasValue)
            {
                var appointment = await _appointmentService.GetFullAppointmemtByIdAsync(dto.AppointmentId.Value);
                var doctor = await _doctorService.GetDoctorByUserIdAsync(appointment.DoctorId);
                dto.Amount = doctor.ConsultationPrice;
                dto.ReferenceId = appointment.UserId;
            }

            if (dto.PaymentFor == PaymentForType.Order)
            {
                var order = await _orderService.GetOrderByIdAsync(dto.OrderId??0);
                    dto.ReferenceId = order.UserId; 

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
