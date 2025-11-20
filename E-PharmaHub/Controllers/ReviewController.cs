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
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost("add-review")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]
        public async Task<IActionResult> AddReview([FromBody] Review review)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (review == null || review.Rating < 1 || review.Rating > 5)
                return BadRequest("Invalid review data.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("User must be logged in to add a review.");

            review.UserId = userId; 

            await _reviewService.AddReviewAsync(review);
            return Ok(new { message = "Review added successfully" });
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] Review updatedReview)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (updatedReview == null || updatedReview.Rating < 1 || updatedReview.Rating > 5)
                return BadRequest("Invalid review data.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User must be logged in to update a review.");

            var updated = await _reviewService.UpdateReviewAsync(id, updatedReview, userId);

            if (!updated)
                return Forbid("You can only update your own reviews or review does not exist.");

            return Ok(new { message = "Review updated successfully." });
        }


        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser,Admin")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User must be logged in to delete a review.");

            var result = await _reviewService.DeleteReviewAsync(id, userId);
            if (!result)
                return Forbid("You can only delete your own reviews.");

            return Ok(new { message = "Review deleted successfully." });
        }

        [HttpGet("pharmacy/{pharmacyId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetPharmacyReviews(int pharmacyId)
        {
            var reviews = await _reviewService.GetReviewsByPharmacyIdAsync(pharmacyId);
            if (reviews == null || !reviews.Any())
                return Ok(new { message = "No reviews found for this pharmacy." });

            return Ok(reviews);
        }

        [HttpGet("medication/{medicationId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetMedicationReviews(int medicationId)
        {
            var reviews = await _reviewService.GetReviewsByMedicationIdAsync(medicationId);
            if (reviews == null || !reviews.Any())
                return Ok(new { message = "No reviews found for this medicine." });

            return Ok(reviews);
        }
        [HttpGet("doctor/{doctorId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetDoctorReviews(int doctorId)
        {
            var reviews = await _reviewService.GetReviewsByDoctorIdAsync(doctorId);
            if (reviews == null || !reviews.Any())
                return Ok(new { message = "No reviews found for this medicine." });

            return Ok(reviews);
        }

    }
}
