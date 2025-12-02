using E_PharmaHub.Dtos;
using E_PharmaHub.Services.AppointmentServ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized(new { message = "User not authorized" });

            dto.UserId = userId; 

            var appointment = await _appointmentService.BookAppointmentAsync(dto);
            return Ok(new
            {
                message = "Appointment booked successfully!",
                appointment
            });
        }

        [HttpGet("doctor")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        public async Task<IActionResult> GetByDoctor()

        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (doctorId == null)
                return Unauthorized(new { message = "Doctor not authorized" });

            var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
            if (!appointments.Any())
                return NotFound(new { message = "No appointments found for this doctor." });

            return Ok(appointments);
        }

        [HttpGet("user")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]
        public async Task<IActionResult> GetByUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "User not authorized" });

            var appointments = await _appointmentService.GetAppointmentsByUserAsync(userId);
            if (!appointments.Any())
                return NotFound(new { message = "No appointments found for this patient." });

            return Ok(appointments);
        } 
        

        [HttpPatch("complete/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        public async Task<IActionResult> CompleteAppointment(int id)
        {
            var result = await _appointmentService.CompleteAppointmentAsync(id);
            if (!result) return NotFound(new { message = "Appointment not found" });

            return Ok(new { message = $"Appointment status updated to Completed" });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpPut("approve/{appointmentId}")]
        public async Task<IActionResult> ApproveAppointment(int appointmentId)
        {
            var (success, message) = await _appointmentService.ApproveAppointmentAsync(appointmentId);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpPut("reject/{appointmentId}")]
        public async Task<IActionResult> RejectAppointment(int appointmentId)
        {
            var (success, message) = await _appointmentService.RejectAppointmentAsync(appointmentId);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

    }
}
