using E_PharmaHub.Models;
using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var addresses = await _addressService.GetAllAddressesAsync();
            if (addresses == null || !addresses.Any())
                return NotFound("No addresses found.");
            return Ok(addresses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var address = await _addressService.GetAddressByIdAsync(id);
            if (address == null)
                return NotFound($"Address with Id {id} not found.");
            return Ok(address);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Pharmacist,Doctor")]

        public async Task<IActionResult> Add([FromBody] Address address)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _addressService.AddAddressAsync(address);
            return CreatedAtAction(nameof(GetById), new { id = address.Id }, address);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Pharmacist,Doctor")]

        public async Task<IActionResult> Update(int id, [FromBody] Address address)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);



            var existing = await _addressService.GetAddressByIdAsync(id);
            if (existing == null)
                return NotFound($"Address with Id {id} not found.");
            

            await _addressService.UpdateAddressAsync(id,address);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _addressService.GetAddressByIdAsync(id);
            if (existing == null)
                return NotFound($"Address with Id {id} not found.");

            await _addressService.DeleteAddressAsync(id);
            return NoContent();
        }
    }
}
