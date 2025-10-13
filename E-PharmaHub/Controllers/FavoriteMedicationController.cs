using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]
    public class FavoriteMedicationController : ControllerBase
    {
        private readonly IFavoriteMedicationService _favoriteMedicationService;

        public FavoriteMedicationController(IFavoriteMedicationService favoriteMedicationService)
        {
            _favoriteMedicationService = favoriteMedicationService;
        }

        [HttpPost("{medicationId}/favorite")]
        public async Task<IActionResult> AddToFavorites(int medicationId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _favoriteMedicationService.AddToFavoritesAsync(userId, medicationId);
            return result ? Ok("Added to favorites.") : BadRequest("Already in favorites.");
        }

        [HttpDelete("{medicationId}/favorite")]
        public async Task<IActionResult> RemoveFromFavorites(int medicationId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _favoriteMedicationService.RemoveFromFavoritesAsync(userId, medicationId);
            return result ? Ok("Removed from favorites.") : NotFound("Not found in favorites.");
        }

        [HttpGet("favorites")]
        public async Task<IActionResult> GetUserFavorites()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var favs = await _favoriteMedicationService.GetUserFavoritesAsync(userId);
            return Ok(favs);
        }
    }
}
