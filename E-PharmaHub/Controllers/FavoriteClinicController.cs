using E_PharmaHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteClinicController : ControllerBase
    {
        private readonly FavoriteClinicService _favoriteClinicService;

        public FavoriteClinicController(FavoriteClinicService favoriteClinicService)
        {
            _favoriteClinicService = favoriteClinicService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToFavorites(string userId, int clinicId)
        {
            var result = await _favoriteClinicService.AddToFavoritesAsync(userId, clinicId);
            if (!result) return BadRequest("Clinic is already in favorites.");

            return Ok("Clinic added to favorites successfully.");
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromFavorites(string userId, int clinicId)
        {
            var result = await _favoriteClinicService.RemoveFromFavoritesAsync(userId, clinicId);
            if (!result) return NotFound("Clinic not found in favorites.");

            return Ok("Clinic removed from favorites successfully.");
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserFavorites(string userId)
        {
            var favorites = await _favoriteClinicService.GetUserFavoritesAsync(userId);
            return Ok(favorites);
        }
    }
}
