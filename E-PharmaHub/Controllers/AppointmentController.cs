using E_PharmaHub.Dtos;
using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost("book")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]
        public async Task<IActionResult> BookAppointment([FromBody] AppointmentDto dto)
        {
            var appointment = await _appointmentService.BookAppointmentAsync(dto);
            return Ok(new { message = "Appointment booked successfully!", appointment });
        }
    }
}
