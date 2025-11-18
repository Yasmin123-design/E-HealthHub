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
    public class MedicineController : ControllerBase
    {
        private readonly IMedicineService _medicineService;
        private readonly IPharmacistService _pharmacistService;
        private readonly IInventoryService _inventoryService;

        public MedicineController(IMedicineService medicineService,
            IPharmacistService pharmacistService,
            IInventoryService inventoryService)
        {
            _medicineService = medicineService;
            _pharmacistService = pharmacistService;
            _inventoryService = inventoryService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _inventoryService.GetInventoryItemByIdAsync(id);
            if (item == null)
                return NotFound($"Medicine with ID {id} not found.");

            return Ok(item);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Pharmacist,Admin")]
        public async Task<IActionResult> Add([FromForm] MedicineDto dto, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            var pharmacist = await _pharmacistService.GetPharmacistProfileByUserIdAsync(userId);
            if (pharmacist == null)
                return NotFound(new { message = "Pharmacist profile not found." });

            if (!pharmacist.IsApproved)
                return Forbid("Your account is pending admin approval.");

            var result = await _medicineService.AddMedicineWithInventoryAsync(dto, image, pharmacist.PharmacyId);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }


        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Pharmacist")]
        public async Task<IActionResult> Update(int id, [FromForm] MedicineDto dto, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            int? pharmacyId = null;
            if (!User.IsInRole("Admin"))
            {
                var pharmacist = await _pharmacistService.GetPharmacistProfileByUserIdAsync(userId);
                if (pharmacist == null)
                    return NotFound(new { message = "Pharmacist profile not found." });

                if (!pharmacist.IsApproved)
                    return Forbid("Your account is pending admin approval.");

                pharmacyId = pharmacist.PharmacyId;
            }

            await _medicineService.UpdateMedicineAsync(id, dto, image, pharmacyId);
            return Ok(new { message = "Medicine updated successfully." });
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            int? pharmacyId = null;
            if (!User.IsInRole("Admin"))
            {
                var pharmacist = await _pharmacistService.GetPharmacistProfileByUserIdAsync(userId);
                if (pharmacist == null)
                    return NotFound(new { message = "Pharmacist profile not found." });

                if (!pharmacist.IsApproved)
                    return Forbid("Your account is pending admin approval.");

                pharmacyId = pharmacist.PharmacyId;
            }

            await _medicineService.DeleteMedicineAsync(id, pharmacyId);
            return Ok(new { message = "Medicine deleted successfully." });
        }

        [HttpGet("search/{name}")]
        public async Task<IActionResult> Search(string name)
        {
            var medicines = await _medicineService.SearchMedicinesByNameAsync(name);
            if (!medicines.Any())
                return NotFound($"No medicines found with name '{name}'.");

            return Ok(medicines);
        }

        [HttpGet("{id}/alternatives")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]

        public async Task<IActionResult> GetAlternatives(int id)
        {
            var alternatives = await _inventoryService.GetAlternativeMedicinesAsync(id);
            if (!alternatives.Any())
                return NotFound(new { message = "No alternative medicines found." });

            return Ok(alternatives);
        }

        [HttpGet("pharmacy/{pharmacyId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]

        public async Task<IActionResult> GetByPharmacy(int pharmacyId)
        {
            var items = await _medicineService.GetMedicinesByPharmacyIdAsync(pharmacyId);
            if (!items.Any())
                return NotFound($"No medicines found for pharmacy ID {pharmacyId}.");

            return Ok(items);
        }

     
    }
}
