using E_PharmaHub.Dtos;
using E_PharmaHub.Services.DoctorServ;
using E_PharmaHub.Services.PrescriptionServ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionController(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        [HttpGet("userPrescription/{userId}")]
        public async Task<IActionResult> GetUserPrescriptions(string userId)
        {        

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result =
                await _prescriptionService.GetUserPrescriptionsAsync(userId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreatePrescriptionDto dto)
        {
           
            await _prescriptionService.CreateAsync(dto);

            return Ok(new { message = "Prescription created successfully" });
        }

        [HttpPut("{prescriptionId}/items")]
        public async Task<IActionResult> UpdateItems(
            int prescriptionId,
            [FromBody] List<PrescriptionItemDto> items)
        {
            await _prescriptionService
                .UpdateItemsAsync(prescriptionId, items);

            return Ok(new { message = "Prescription items updated successfully" });
        }

        [HttpDelete("{prescriptionId}")]
        public async Task<IActionResult> Delete(int prescriptionId)
        {
            await _prescriptionService.DeleteAsync(prescriptionId);

            return Ok(new { message = "Prescription deleted successfully" });
        }

        [HttpPost("{prescriptionId}/items")]
        public async Task<IActionResult> AddItem(
            int prescriptionId,
            [FromBody] PrescriptionItemDto dto)
        {
            await _prescriptionService
                .AddItemAsync(prescriptionId, dto);

            return Ok(new { message = "Item added successfully" });
        }

        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateItem(
            int itemId,
            [FromBody] PrescriptionItemDto dto)
        {
            await _prescriptionService
                .UpdateItemAsync(itemId, dto);

            return Ok(new { message = "Item updated successfully" });
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> DeleteItem(int itemId)
        {
            await _prescriptionService.DeleteItemAsync(itemId);

            return Ok(new { message = "Item deleted successfully" });
        }

    }
}
