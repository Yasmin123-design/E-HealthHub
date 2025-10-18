using E_PharmaHub.Dtos;
using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IDoctorService _doctorService;
        public PrescriptionController(IPrescriptionService prescriptionService,
            IDoctorService doctorService)
        {
            _prescriptionService = prescriptionService;
            _doctorService = doctorService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var doctorUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorUserId == null)
                return Unauthorized();

            var doctor = await _doctorService.GetDoctorByUserIdAsync(doctorUserId);
            if (doctor == null)
                return BadRequest(new { message = "Doctor profile not found" });

            dto.DoctorId = doctor.Id;

            var result = await _prescriptionService.CreatePrescriptionAsync(dto);
            if (!result.success)
                return BadRequest(new { message = result.message });

            return Ok(new { message = "Prescription created successfully"});
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]
        [HttpGet("user")]
        public async Task<IActionResult> GetUserPrescriptions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var prescriptions = await _prescriptionService.GetUserPrescriptionsAsync(userId);
            return Ok(prescriptions);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpGet("doctor")]
        public async Task<IActionResult> GetDoctorPrescriptions()
        {

            var doctorUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorUserId == null)
                return Unauthorized();

            var doctor = await _doctorService.GetDoctorByUserIdAsync(doctorUserId);
            if (doctor == null)
                return BadRequest(new { message = "Doctor profile not found" });

            var prescriptions = await _prescriptionService.GetDoctorPrescriptionsAsync(doctor.Id);
            return Ok(prescriptions);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrescriptionById(int id)
        {
            var prescription = await _prescriptionService.GetByIdAsync(id);
            if (prescription == null)
                return NotFound(new { message = "Prescription not found" });

            return Ok(prescription);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var result = await _prescriptionService.DeletePrescriptionAsync(id);
            if (!result)
                return BadRequest(new { message = "Failed to delete prescription" });

            return Ok(new { message = "Prescription deleted successfully" });
        }

    }
}
