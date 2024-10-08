using MegStore.Application.DTOs;
using MegStore.Core.Entities.Users;
using MegStore.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MegStore.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
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
            var message = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset PIN Code</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            background: white;
            padding: 20px;
            border-radius: 5px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background-color: #007BFF;
            color: white;
            padding: 10px 20px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        h2 {{
            color: #333;
        }}
        p {{
            color: #555;
            line-height: 1.5;
        }}
        .footer {{
            margin-top: 30px;
            font-size: 12px;
            color: #888;
            text-align: center;
        }}
        .pin-code {{
            font-size: 24px;
            font-weight: bold;
            color: #007BFF;
        }}
        .contact {{
            margin-top: 10px;
            font-size: 14px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>MegStore</h1>
        </div>
        <h2>Password Reset Request</h2>
        <p>Dear {user.fullName},</p>
        <p>We received a request to reset your password. Your password reset PIN code is:</p>
        <p class='pin-code'>{pinCode}</p>
        <p>This code is valid for 15 minutes. If you did not request this, please ignore this email.</p>
        <p>If you have any questions or need further assistance, feel free to contact our support team.</p>
        <p>Thank you,<br>The MegStore Team</p>
        <div class='footer'>
            <p class='contact'>Contact us: support@megstore.com | (123) 456-7890</p>
            <p>This email was sent from your MegStore application.</p>
        </div>
    </div>
</body>
</html>";

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

