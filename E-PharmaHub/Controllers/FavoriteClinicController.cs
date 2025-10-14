using E_PharmaHub.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteClinicController : ControllerBase
    {
        private readonly IFavoriteClinicService _favoriteClinicService;

        public FavoriteClinicController(IFavoriteClinicService favoriteClinicService)
        {
            _favoriteClinicService = favoriteClinicService;
        }

        [HttpPost("{clinicId}/favorite")]
        public async Task<IActionResult> AddToFavorites(int clinicId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _favoriteClinicService.AddToFavoritesAsync(userId, clinicId);
            return result ? Ok("Added to favorites.") : BadRequest("Already in favorites.");
        }

        [HttpDelete("{clinicId}/favorite")]
        public async Task<IActionResult> RemoveFromFavorites(int clinicId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _favoriteClinicService.RemoveFromFavoritesAsync(userId, clinicId);
            return result ? Ok("Removed from favorites.") : NotFound("Not found in favorites.");
        }

        [HttpGet("favorites")]
        public async Task<IActionResult> GetUserFavorites()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var favs = await _favoriteClinicService.GetUserFavoritesAsync(userId);
            return Ok(favs);
        }
    }
}
