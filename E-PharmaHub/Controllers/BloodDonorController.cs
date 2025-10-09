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
    public class BloodDonorController : ControllerBase
    {
        private readonly IDonorService _donorService;

        public BloodDonorController(IDonorService donorService)
        {
            _donorService = donorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var donors = await _donorService.GetAllDetailsAsync();
            return Ok(donors);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] BloodType? type, [FromQuery] string? city)
        {
            var donors = await _donorService.GetByFilterAsync(type, city);
            return Ok(donors);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DonorRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _donorService.RegisterAsync(dto);
                return Ok(new
                {
                    message = "Donor registered successfully! Awaiting admin approval.",
                    userId = result.AppUserId,
                    email = result.AppUser.Email,
                    role = result.AppUser.Role.ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpPut("availability")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateAvailability([FromBody] bool isAvailable)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _donorService.UpdateAvailabilityAsync(userId, isAvailable);
            if (!success) return NotFound("Donor not found.");
            return Ok("Availability updated successfully.");
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _donorService.DeleteAsync(id);
            return Ok("Donor deleted successfully");
        }
    }
}
