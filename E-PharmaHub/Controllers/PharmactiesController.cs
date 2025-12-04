using E_PharmaHub.Dtos;
using E_PharmaHub.Services.PharmacistServ;
using E_PharmaHub.Services.PharmacyServ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmactiesController : ControllerBase
    {
        private readonly IPharmacistService _pharmacistService;
        private readonly IPharmacyService _pharmacyService;
        public PharmactiesController(IPharmacistService pharmacistService,IPharmacyService pharmacyService)
        {
            _pharmacistService = pharmacistService;
            _pharmacyService = pharmacyService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] PharmacistRegisterDto dto, IFormFile pharmacyImage , IFormFile pharmacistImage)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _pharmacistService.RegisterPharmacistAsync(dto, pharmacyImage,pharmacistImage);

                return Ok(new
                {
                    message = "Pharmacist registered successfully! Awaiting admin approval.",
                    name = user.UserName,
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Pharmacist")]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] PharmacistUpdateDto dto, IFormFile? image)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _pharmacistService.UpdatePharmacistProfileAsync(userId, dto, image);

                if (!result)
                    return NotFound(new { message = "Pharmacist profile not found." });

                return Ok(new { message = "Pharmacist profile updated successfully ✅" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Pharmacist")]
        [HttpPut("update-pharmacy")]
        public async Task<IActionResult> UpdatePharmacy([FromForm] PharmacyUpdateDto dto, IFormFile? image)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            var pharmacistProfile = await _pharmacistService.GetPharmacistProfileByUserIdAsync(userId);
            if (pharmacistProfile == null)
                return NotFound(new { message = "Pharmacist profile not found." });

            if (pharmacistProfile.PharmacyId == null)
                return BadRequest(new { message = "You are not assigned to any pharmacy." });

            int pharmacyId = pharmacistProfile.PharmacyId;

            var (success, message) = await _pharmacyService.UpdatePharmacyAsync(pharmacyId, dto, image);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }





        [HttpGet("all")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<IActionResult> GetAllPharmacists()
        {
            var pharmacists = await _pharmacistService.GetAllPharmacistsAsync();
            return Ok(pharmacists);
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<IActionResult> GetPharmacistById(int id)
        {
            var pharmacist = await _pharmacistService.GetPharmacistByIdAsync(id);
            if (pharmacist == null)
                return NotFound(new { message = "Pharmacist not found." });

            return Ok(pharmacist);
        }


        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<IActionResult> DeletePharmacist(int id)
        {
            try
            {
                await _pharmacistService.DeletePharmacistAsync(id);
                return Ok(new { message = "Pharmacist deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApprovePharmacist(int id)
        {
            var (success, message) = await _pharmacistService.ApprovePharmacistAsync(id);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectPharmacist(int id)
        {
            var (success, message) = await _pharmacistService.RejectPharmacistAsync(id);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }




    }

}
