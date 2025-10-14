using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_PharmaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;

        public UserController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration config,
            IEmailSender emailSender,
            IUserService userService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _emailSender = emailSender;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminEmail = _config["AdminSettings:AdminEmail"];

            if (model.Email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase))
            {
                model.Role = UserRole.Admin;
            }
            else
            {
                if (model.Role != UserRole.RegularUser)
                    return BadRequest("Invalid role selection. You can only register as RegularUser");
            }

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                Role = model.Role,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, model.Role.ToString());

            return Ok(new
            {
                message = "User registered successfully!",
                role = user.Role.ToString()
            });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("Invalid credentials , email not found");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded) return Unauthorized("Invalid credentials , wrong password");

            var roles = await _userManager.GetRolesAsync(user);

            var token = GenerateJwtToken(user, roles);

            return Ok(new
            {
                token,
                user = new { user.UserName, user.Email, Roles = roles }
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return BadRequest("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"{_config["Jwt:Audience"]}/reset-password?email={user.Email}&token={Uri.EscapeDataString(token)}";

            await _emailSender.SendEmailAsync(user.Email, "Reset Your Password",
                $"<p>Click the link below to reset your password:</p><a href='{resetLink}'>Reset Password</a>");

            return Ok(new { message = "Password reset link has been sent to your email.",token });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return BadRequest("User not found");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { message = "Password has been reset successfully!" });
        }
        private string GenerateJwtToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("FullName", user.UserName)
        };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, "Google");
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (!result.Succeeded) return BadRequest("Google login failed");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    Role = UserRole.RegularUser
                };
                await _userManager.CreateAsync(user);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            return Ok(new { token, user = new { user.UserName, user.Email, Roles = roles } });
        }

        [HttpGet("facebook-login")]
        public IActionResult FacebookLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("FacebookResponse") };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [HttpGet("facebook-response")]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (!result.Succeeded) return BadRequest("Facebook login failed");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    Role = UserRole.RegularUser
                };
                await _userManager.CreateAsync(user);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            return Ok(new { token, user = new { user.UserName, user.Email, Roles = roles } });
        }

        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]

        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var profile = await _userService.GetUserProfileAsync(userId);

            if (profile == null)
                return NotFound(new { message = "User not found ❌" });

            return Ok(profile);
        }

        [HttpPut("update-profile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]

        public async Task<IActionResult> UpdateProfile([FromForm] UserUpdateDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var (success, message) = await _userService.UpdateUserProfileAsync(userId, dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPost("upload-picture")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]

        public async Task<IActionResult> UploadProfilePicture(IFormFile image)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var (success, message) = await _userService.UploadProfilePictureAsync(userId, image);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [HttpPut("update-picture")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "RegularUser")]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile newImage)
        {
            if (newImage == null || newImage.Length == 0)
                return BadRequest(new { message = "Please upload a valid image 🖼️⚠️" });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var (success, message) = await _userService.UpdateProfilePictureAsync(userId, newImage);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

    }
}
