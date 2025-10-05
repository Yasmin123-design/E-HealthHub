using E_PharmaHub.Models;
using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyService _pharmacyService;

        public PharmacyController(IPharmacyService pharmacyService)
        {
            _pharmacyService = pharmacyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pharmacies = await _pharmacyService.GetAllPharmaciesAsync();
            if (pharmacies == null || !pharmacies.Any())
                return NotFound("No pharmacies found.");

            return Ok(pharmacies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pharmacy = await _pharmacyService.GetPharmacyByIdAsync(id);
            if (pharmacy == null)
                return NotFound($"Pharmacy with Id {id} not found.");

            return Ok(pharmacy);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "Admin")]
        public async Task<IActionResult> Add([FromForm] Pharmacy pharmacy, IFormFile image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); 

            if (pharmacy == null)
                return BadRequest("Pharmacy data is required.");

            await _pharmacyService.AddPharmacyAsync(pharmacy,image);
            return CreatedAtAction(nameof(GetById), new { id = pharmacy.Id }, pharmacy);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "Admin,Pharmacist")]

        public async Task<IActionResult> Update(int id, [FromForm] Pharmacy pharmacy , IFormFile image)
        {
            if (pharmacy == null)
                return BadRequest("Pharmacy data is invalid.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState); 

            var existing = await _pharmacyService.GetPharmacyByIdAsync(id);
            if (existing == null)
                return NotFound($"Pharmacy with Id {id} not found.");

            await _pharmacyService.UpdatePharmacyAsync(id , pharmacy, image);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _pharmacyService.GetPharmacyByIdAsync(id);
            if (existing == null)
                return NotFound($"Pharmacy with Id {id} not found.");

            await _pharmacyService.DeletePharmacyAsync(id);
            return NoContent();
        }
    }
}
