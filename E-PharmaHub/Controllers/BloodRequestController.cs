using E_PharmaHub.Models;
using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BloodRequestController : ControllerBase
    {
        private readonly IBloodRequestService _bloodRequestService;

        public BloodRequestController(IBloodRequestService bloodRequestService)
        {
            _bloodRequestService = bloodRequestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bloodRequestService.GetAllRequestsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _bloodRequestService.GetRequestByIdAsync(id);
            if (request == null) return NotFound();
            return Ok(request);
        }

        [HttpGet("unfulfilled")]
        public async Task<IActionResult> GetUnfulfilled()
        {
            var result = await _bloodRequestService.GetUnfulfilledRequestsAsync();
            return Ok(result);
        }

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> Create([FromBody] BloodRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            request.RequestedByUserId = userId;

            var newRequest = await _bloodRequestService.AddRequestAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = newRequest.Id }, newRequest);
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] BloodRequest updatedRequest)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var existing = await _bloodRequestService.GetRequestByIdAsync(id);
            if (existing == null) return NotFound("Blood request not found.");

            if (existing.RequestedByUserId != userId)
                return Forbid("You are not allowed to update this request.");

            var success = await _bloodRequestService.UpdateRequestAsync(id, updatedRequest);
            if (!success) return BadRequest("Failed to update request.");

            return Ok("Updated successfully");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _bloodRequestService.DeleteRequestAsync(id);
            if (!success) return NotFound();
            return Ok("Deleted successfully");
        }
    }

}
