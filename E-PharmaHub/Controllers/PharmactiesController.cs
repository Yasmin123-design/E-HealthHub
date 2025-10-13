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
    public class PharmactiesController : ControllerBase
    {
        private readonly IPharmacistService _pharmacistService;
        private readonly IEmailSender _emailSender;
        public PharmactiesController(IPharmacistService pharmacistService,IEmailSender emailSender)
        {
            _pharmacistService = pharmacistService;
            _emailSender = emailSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] PharmacistRegisterDto dto, IFormFile image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _pharmacistService.RegisterPharmacistAsync(dto, image);

                return Ok(new
                {
                    message = "Pharmacist registered successfully! Awaiting admin approval.",
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Pharmacist")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePharmacist(int id, [FromForm] PharmacistProfile updated, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            try
            {
                if (!User.IsInRole("Admin"))
                {
                    var pharmacist = await _pharmacistService.GetPharmacistByUserIdAsync(userId);
                    if (pharmacist == null)
                        return NotFound(new { message = "Pharmacist profile not found." });

                    if (!pharmacist.IsApproved)
                        return Forbid("Your account is pending admin approval.");
                }

                await _pharmacistService.UpdatePharmacistAsync(id, updated, image);
                return Ok(new { message = "Pharmacist updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
