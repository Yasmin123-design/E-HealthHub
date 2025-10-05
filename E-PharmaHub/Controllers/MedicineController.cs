using E_PharmaHub.Models;
using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicineController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var medicines = await _medicineService.GetAllMedicinesAsync();

            if (!medicines.Any())
                return NotFound("No medicines found.");

            return Ok(medicines);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var medicine = await _medicineService.GetMedicineByIdAsync(id);
            if (medicine == null)
                return NotFound($"Medicine with ID {id} not found.");
            return Ok(medicine);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Pharmacist,Admin")]
        public async Task<IActionResult> Add([FromForm] Medication medicine, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (medicine == null)
                return BadRequest("Medicine data is required.");

            await _medicineService.AddMedicineAsync(medicine, image);
            return CreatedAtAction(nameof(GetById), new { id = medicine.Id }, medicine);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Pharmacist,Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] Medication medicine, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (medicine == null)
                return BadRequest("Invalid medicine data.");

            await _medicineService.UpdateMedicineAsync(id,medicine, image);
            return Ok(new { message = "Medicine updated successfully", medicine });
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            var medicine = await _medicineService.GetMedicineByIdAsync(id);
            if (medicine == null)
                return NotFound($"Medicine with ID {id} not found.");

            await _medicineService.DeleteMedicineAsync(id);

            return Ok("Medicine deleted successfully.");
        }

        [HttpGet("search/{name}")]
        public async Task<IActionResult> Search(string name)
        {
            var result = await _medicineService.SearchMedicinesByNameAsync(name);
            if (!result.Any())
                return NotFound($"No medicines found with name containing '{name}'.");

            return Ok(result);
        }

        [HttpGet("pharmacy/{pharmacyId}")]
        public async Task<IActionResult> GetByPharmacy(int pharmacyId)
        {
            var result = await _medicineService.GetMedicinesByPharmacyIdAsync(pharmacyId);
            if (!result.Any())
                return NotFound($"No medicines found for pharmacy ID {pharmacyId}.");

            return Ok(result);
        }
        [HttpGet("nearest")]
        public async Task<IActionResult> GetNearestPharmacies(string medicationName, double lat, double lng)
        {
            var pharmacies = await _medicineService.GetNearestPharmaciesWithMedicationAsync(medicationName, lat, lng);
            if (!pharmacies.Any())
                return NotFound($"No pharmacies found with medication '{medicationName}' near your location.");

            return Ok(pharmacies);
        }
    }
}
