using E_PharmaHub.Dtos;
using E_PharmaHub.Services.OrderServ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not found");

            var result = await _orderService.CheckoutAsync(userId, dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingOrder([FromQuery] int pharmacyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not found");

            var order = await _orderService.GetPendingOrderByUserAsync(userId, pharmacyId);

            if (order == null)
                return NotFound("No pending order found for this pharmacy.");

            return Ok(order);
        }
        [HttpGet("user-orders")]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("User not found");

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }
    }
}
