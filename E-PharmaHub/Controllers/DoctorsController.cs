using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] DoctorRegisterDto dto, IFormFile image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _doctorService.RegisterDoctorAsync(dto, image);

                return Ok(new
                {
                    message = "Doctor registered successfully! Awaiting admin approval.",
                    userId = user.Id,
                    email = user.Email,
                    role = user.Role.ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            return Ok(doctor);
        }

        [HttpGet("specialty/{specialty}")]
        public async Task<IActionResult> GetDoctorsBySpecialty(string specialty)
        {
            var doctors = await _doctorService.GetDoctorsBySpecialtyAsync(specialty);
            return Ok(doctors);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctorWithClinicWithAddressRelated(int id, [FromForm] DoctorProfile doctor, IFormFile? image)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            var existingDoctor = await _doctorService.GetDoctorByUserIdAsync(userId);
            if (existingDoctor == null)
                return NotFound(new { message = "Doctor profile not found." });

            if (!existingDoctor.IsApproved)
                return Forbid("Your account is pending admin approval.");

            doctor.AppUserId = userId;
            ModelState.Remove("AppUserId");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _doctorService.UpdateDoctorAsync(id, doctor, image);
                return Ok(new { message = "Doctor updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<IActionResult> DeleteDoctorWithClinicWithAddressRelated(int id)
        {
            try
            {
                await _doctorService.DeleteDoctorAsync(id);
                return Ok(new { message = "Doctor deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveDoctor(int id)
        {
            var result = await _doctorService.ApproveDoctorAsync(id);
            if (!result)
                return NotFound(new { message = "Doctor not found or already approved." });

            return Ok(new { message = "Doctor approved successfully." });
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectDoctor(int id)
        {
            var result = await _doctorService.RejectDoctorAsync(id);
            if (!result)
                return NotFound(new { message = "Doctor not found or already rejected." });

            return Ok(new { message = "Doctor rejected successfully." });
        }

    }
}
