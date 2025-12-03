using E_PharmaHub.Services.NotificationServ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet("user")]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var (orders, appointments) = await _notificationService.GetUserNotificationsByCategoryAsync(userId);

            return Ok(new
            {
                Orders = orders,
                Appointments = appointments
            });
        }


    }
}
