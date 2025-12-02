using E_PharmaHub.Models;
using E_PharmaHub.Services.AddressServ;
using E_PharmaHub.Services.DoctorServ;
using E_PharmaHub.Services.PharmacistServ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly IDoctorService _doctorService;
        private readonly IPharmacistService _pharmacistService;

        public AddressController(IAddressService addressService,IDoctorService doctorService , IPharmacistService pharmacistService)
        {
            _addressService = addressService;
            _doctorService = doctorService;
            _pharmacistService = pharmacistService;
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

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            if (User.IsInRole("Admin"))
            {
                await _addressService.AddAddressAsync(address);
                return CreatedAtAction(nameof(GetById), new { id = address.Id }, address);
            }

            bool isApproved = false;

            if (User.IsInRole("Doctor"))
            {
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                if (doctor == null)
                    return NotFound(new { message = "Doctor profile not found." });

                isApproved = doctor.IsApproved;
            }
            else if (User.IsInRole("Pharmacist"))
            {
                var pharmacist = await _pharmacistService.GetPharmacistByUserIdAsync(userId);
                if (pharmacist == null)
                    return NotFound(new { message = "Pharmacist profile not found." });

                isApproved = pharmacist.IsApproved;
            }

            if (!isApproved)
                return Forbid("Your account is pending admin approval.");

            await _addressService.AddAddressAsync(address);
            return CreatedAtAction(nameof(GetById), new { id = address.Id }, address);
        }


        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Pharmacist,Doctor")]
        public async Task<IActionResult> Update(int id, [FromBody] Address address)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated." });

            if (User.IsInRole("Admin"))
            {
                var existing = await _addressService.GetAddressByIdAsync(id);
                if (existing == null)
                    return NotFound($"Address with Id {id} not found.");

                await _addressService.UpdateAddressAsync(id, address);
                return NoContent();
            }

            bool isApproved = false;

            if (User.IsInRole("Doctor"))
            {
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                if (doctor == null)
                    return NotFound(new { message = "Doctor profile not found." });

                isApproved = doctor.IsApproved;
            }
            else if (User.IsInRole("Pharmacist"))
            {
                var pharmacist = await _pharmacistService.GetPharmacistByUserIdAsync(userId);
                if (pharmacist == null)
                    return NotFound(new { message = "Pharmacist profile not found." });

                isApproved = pharmacist.IsApproved;
            }

            if (!isApproved)
                return Forbid("Your account is pending admin approval.");

            var existingAddress = await _addressService.GetAddressByIdAsync(id);
            if (existingAddress == null)
                return NotFound($"Address with Id {id} not found.");

            await _addressService.UpdateAddressAsync(id, address);
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
