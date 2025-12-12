using E_PharmaHub.Dtos;
using E_PharmaHub.Models.Enums;
using E_PharmaHub.Services.ClinicServ;
using E_PharmaHub.Services.DoctorServ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IClinicService _clinicService;

        public DoctorsController(IDoctorService doctorService,IClinicService clinicService
            )
        {
            _doctorService = doctorService;
            _clinicService = clinicService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] DoctorRegisterDto dto,
            IFormFile? clinicImage,
            IFormFile doctorImage
            )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _doctorService.RegisterDoctorAsync(dto, clinicImage,doctorImage);

                return Ok(new
                {
                    message = "Doctor registered successfully! Awaiting admin approval.",
                    userId = user.Id,
                    email = user.Email,
                    name = user.UserName,
                    role = user.Role.ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpPut("update-clinic")]
        public async Task<IActionResult> UpdateClinic([FromForm] ClinicUpdateDto dto, IFormFile? image)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            var (success, message) = await _clinicService.UpdateClinicAsync(userId, dto, image);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] DoctorUpdateDto dto, IFormFile? doctorImage)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var result = await _doctorService.UpdateDoctorProfileAsync(userId, dto, doctorImage);

                if (!result)
                    return NotFound(new { message = "Doctor profile not found." });

                return Ok(new { message = "Doctor profile updated successfully ✅" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var doctor = await _doctorService.GetByIdDetailsAsync(id);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            return Ok(doctor);
        }

        [HttpGet("allDoctorsShowToRegularUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]

        public async Task<IActionResult> GetAllDoctorsShowToRegularUser()
        {
            var doctor = await _doctorService.GetAllDoctorsAcceptedByAdminAsync();
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            return Ok(doctor);
        }

        [HttpGet("allDoctorsShowToAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<IActionResult> GetAllDoctorsShowToAdmin()
        {
            var doctor = await _doctorService.GetAllDoctorsShowToAdmin();
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            return Ok(doctor);
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
            var (success, message) = await _doctorService.ApproveDoctorAsync(id);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectDoctor(int id)
        {
            var (success, message) = await _doctorService.RejectDoctorAsync(id);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }


        [HttpGet("filterDoctors")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]

        public async Task<IActionResult> GetDoctors(
            [FromQuery] string? specialty,
            [FromQuery] string? name,
            [FromQuery] Gender? gender,
            [FromQuery] string? sort,
            [FromQuery] ConsultationType? consultationType)
        {
            var doctors = await _doctorService.GetDoctorsAsync(specialty, name, gender, sort, consultationType);
            return Ok(doctors);
        }


        [HttpGet("top-doctors")]
        public async Task<IActionResult> GetTopDoctors()
        {
            var result = await _doctorService.GetTopRatedDoctorsAsync();
            return Ok(result);
        }


    }
}
