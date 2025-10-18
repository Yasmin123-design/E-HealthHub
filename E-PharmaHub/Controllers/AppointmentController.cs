using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctor(string doctorId)
        {
            var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
            if (!appointments.Any())
                return NotFound(new { message = "No appointments found for this doctor." });

            return Ok(appointments);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var appointments = await _appointmentService.GetAppointmentsByUserAsync(userId);
            if (!appointments.Any())
                return NotFound(new { message = "No appointments found for this patient." });

            return Ok(appointments);
        } 
        

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] AppointmentStatus status)
        {
            var result = await _appointmentService.UpdateStatusAsync(id, status);
            if (!result) return NotFound(new { message = "Appointment not found" });

            return Ok(new { message = $"Appointment status updated to {status}" });
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
