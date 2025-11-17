using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavouriteDoctorController : ControllerBase
    {
        private readonly IDoctorFavouriteService _doctorFavouriteService;

        public FavouriteDoctorController(IDoctorFavouriteService doctorFavouriteService)
        {
            _doctorFavouriteService = doctorFavouriteService;
        }

        [HttpGet("user")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]
        public async Task<IActionResult> GetUserFavorites()
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var favorites = await _doctorFavouriteService.GetUserFavoritesAsync(userId);
            return Ok(favorites);
        }

        [HttpPost("add/{doctorId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]
        public async Task<IActionResult> AddToFavorites(int doctorId)
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _doctorFavouriteService.AddToFavoritesAsync(userId, doctorId);
            if (!success)
                return BadRequest("Could not add doctor to favorites.");

            return Ok("Doctor added to favorites successfully.");
        }

        [HttpDelete("remove/{doctorId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]
        public async Task<IActionResult> RemoveFromFavorites(int doctorId)
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _doctorFavouriteService.RemoveFromFavoritesAsync(userId, doctorId);
            if (!success)
                return BadRequest("Could not remove doctor from favorites.");

            return Ok("Doctor removed from favorites successfully.");
        }
    }
}
