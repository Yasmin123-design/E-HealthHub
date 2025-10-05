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
    public class PharmactiesController : ControllerBase
    {
        private readonly IPharmacistService _pharmacistService;

        public PharmactiesController(IPharmacistService pharmacistService)
        {
            _pharmacistService = pharmacistService;
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
                return BadRequest(new { error = ex.Message });
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

        [HttpPost("add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> AddPharmacist([FromBody] PharmacistProfile pharmacist)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _pharmacistService.AddPharmacistAsync(pharmacist);
            return Ok(new { message = "Pharmacist added successfully." });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Pharmacist")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePharmacist(int id, [FromBody] PharmacistProfile updated)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _pharmacistService.UpdatePharmacistAsync(id, updated);
                return Ok(new { message = "Pharmacist updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Pharmacist")]

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

    }

}
