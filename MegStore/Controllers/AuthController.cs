using MegStore.Application.DTOs;
using MegStore.Core.Entities.Users;
using MegStore.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MegStore.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public AuthController(
           UserManager<User> userManager,
           SignInManager<User> signInManager,
           IConfiguration configuration,
           IEmailService emailService)
        {
            _emailService = emailService;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
           
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            User user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                fullName = model.FullName,
                dateOfbirth = model.DateOfBirth,
                PhoneNumber = model.PhoneNumber,
                address = model.Address,
                gender = model.Gender,
                role=model.Role,
                dateOfCreation=model.dateOfCreation,
                userStatus = UserStatus.Active 

            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }

       
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new { Token = GenerateJwtToken(user, roles) });
            }

            return Unauthorized(new { message = "Invalid login attempt." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Successfully logged out." });
        }

        [HttpPost("change-password/{userId}")]
        public async Task<IActionResult> ChangePassword(long userId, [FromBody] ChangePasswordModel model)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) return NotFound("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded) return Ok("Password changed successfully.");

            return BadRequest(result.Errors);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return NotFound("User not found.");

            var pinCode = new Random().Next(100000, 999999).ToString();
            user.PasswordResetCode = pinCode;
            user.PasswordResetCodeExpiration = DateTime.UtcNow.AddMinutes(15);

            await _userManager.UpdateAsync(user);

            var subject = "Password Reset PIN Code";
            var message = $"Your password reset PIN code is: {pinCode}";
            await _emailService.SendEmailAsync(user.Email, subject, message);

            return Ok(new { message = "Password reset PIN code has been sent to your email." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return NotFound("User not found.");

            if (user.PasswordResetCode != model.PinCode || user.PasswordResetCodeExpiration < DateTime.UtcNow)
                return BadRequest("Invalid or expired PIN code.");

            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded) return BadRequest(removePasswordResult.Errors);

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (addPasswordResult.Succeeded)
            {
                user.PasswordResetCode = null;
                user.PasswordResetCodeExpiration = null;
                await _userManager.UpdateAsync(user);

                return Ok("Password has been reset successfully.");
            }

            return BadRequest(addPasswordResult.Errors);
        }

        private string GenerateJwtToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Id.ToString()),
                new Claim("fullName", user.fullName),
                new Claim("role", user.role.ToString()),
          
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

