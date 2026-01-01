using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Services.PharmacistServ;
using E_PharmaHub.Services.PharmacyServ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyService _pharmacyService;
        private readonly IPharmacistService _pharmacistService;

        public PharmacyController(IPharmacyService pharmacyService,IPharmacistService pharmacistService)
        {
            _pharmacyService = pharmacyService;
            _pharmacistService = pharmacistService;
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
        [HttpPut]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Pharmacist"
        )]
        public async Task<IActionResult> Update(
            [FromForm] PharmacyUpdateDto dto,
            IFormFile? image)
        {
            if (dto == null)
                return BadRequest("Pharmacy data is invalid.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            int pharmacyId;

            var pharmacyIdResult =
                await _pharmacistService.GetPharmacyIdByUserIdAsync(userId);

            if (!pharmacyIdResult.HasValue)
                return NotFound("Pharmacy not found for this pharmacist.");

            pharmacyId = pharmacyIdResult.Value;

            var existing = await _pharmacyService.GetPharmacyByIdAsync(pharmacyId);

            if (existing == null)
                return NotFound($"Pharmacy with Id {pharmacyId} not found.");

            try
            {
                await _pharmacyService.UpdatePharmacyAsync(pharmacyId, dto, image);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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


        [HttpGet("nearest")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]

        public async Task<IActionResult> GetNearestPharmacies(string medicationName, double lat, double lng)
        {
            var pharmacies = await _pharmacyService.GetNearestPharmaciesWithMedicationAsync(medicationName, lat, lng);
            if (!pharmacies.Any())
                return NotFound($"No pharmacies found with medication '{medicationName}' near your location.");

            return Ok(pharmacies);
        }

        [HttpGet("top-pharmacies")]
        public async Task<IActionResult> GetTopPharmacies()
        {
            var result = await _pharmacyService.GetTopRatedPharmaciesAsync();
            return Ok(result);
        }

    }
}
