using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Route("api/pharmacist/orders")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Pharmacist")]
    public class PharmacistOrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPharmacistService _pharmacistService;

        public PharmacistOrderController(
            IOrderService orderService,
            IPharmacistService pharmacistService
            )
        {
            _orderService = orderService;
            _pharmacistService = pharmacistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersForMyPharmacy()
        {
            var pharmacistId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (pharmacistId == null)
                return Unauthorized("Pharmacist not found.");

            var pharmacist = await _pharmacistService.GetPharmacistByUserIdAsync(pharmacistId);
            if (pharmacist == null || pharmacist.PharmacyId == null)
                return BadRequest("No pharmacy found for this pharmacist.");

            var orders = await _orderService.GetOrdersByPharmacyIdAsync(pharmacist.PharmacyId);
            if (orders == null || !orders.Any())
                return NotFound("No orders found for this pharmacy.");

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var pharmacistId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = await _pharmacistService.GetPharmacistByUserIdAsync(pharmacistId);

            if (pharmacist == null || pharmacist.PharmacyId == null)
                return BadRequest("No pharmacy found for this pharmacist.");

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.PharmacyId != pharmacist.PharmacyId)
                return NotFound("Order not found or does not belong to your pharmacy.");

            return Ok(order);
        }

        [HttpPut("{id}/accept")]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            var pharmacistId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = await _pharmacistService.GetPharmacistByUserIdAsync(pharmacistId);

            if (pharmacist == null || pharmacist.PharmacyId == null)
                return BadRequest("No pharmacy found for this pharmacist.");

            var result = await _orderService.AcceptOrderAsync(id);
            if (!result)
                return BadRequest("Failed to accept order.");

            return Ok("Order accepted successfully.");
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var pharmacistId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pharmacist = await _pharmacistService.GetPharmacistByUserIdAsync(pharmacistId);

            if (pharmacist == null || pharmacist.PharmacyId == null)
                return BadRequest("No pharmacy found for this pharmacist.");

            var result = await _orderService.CancelOrderAsync(id);
            if (!result)
                return BadRequest("Failed to cancel order.");

            return Ok("Order cancelled successfully.");
        }
    }
}

