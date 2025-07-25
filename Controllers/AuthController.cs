using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Software_Solutions.Data;
using Software_Solutions.DTO;
using Software_Solutions.Models;
using Software_Solutions.Services;
using System.Security.Claims;

namespace Software_Solutions.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthControlloer : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailServices _emailServices;
        private readonly TokenService _tokenService;

        public AuthControlloer(AppDbContext context, EmailServices emailServices, TokenService tokenService)
        {
            _context = context;
            _emailServices = emailServices;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            if (_context.Users.Any(u => u.Email == registerDto.Email || u.PhoneNumber == registerDto.PhoneNumber))
                return BadRequest("User Email or Phone Number already exists. ");

            if (registerDto.Password != registerDto.ConfirmPassword) return BadRequest("Password and Confirm not matched.");

            string hasedPass = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            string token = _tokenService.GenerateEmailVerficationToken();

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                PasswordHash = hasedPass,
                EmailVerficationToken = token,
                TokenExpiration = DateTime.UtcNow.AddHours(1),
                EmailConfirmed = false

            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            string verifyLink = $"{Request.Scheme}://{Request.Host}/api/AuthControlloer/verify?token={token}";

            string body = $"<p>Click the link to verify your email: <a href='{verifyLink}'>Verify</a></p>";

            await _emailServices.SendEmailAsync(registerDto.Email, "Verify Your Account", body);

            return Ok("Account Created Successufully. Please Check your email to verify your account. ");
        }

        [HttpGet("verify")]
        public async Task<ActionResult> VerifyEmail(string token)
        {
            var user = _context.Users.FirstOrDefault(u => u.EmailVerficationToken == token);
            if (user == null || user.TokenExpiration < DateTime.UtcNow)
                return BadRequest("Invalid or Expired verification token");

            user.EmailConfirmed = true;
            user.EmailVerficationToken = null;
            user.TokenExpiration = DateTime.MinValue;

            await _context.SaveChangesAsync();
            return Ok("Email Verified Succuessfully, You're now login in");


        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto lDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
            (u.Email == lDto.Identifier || u.PhoneNumber == lDto.Identifier));

            if (user == null) return Unauthorized("User not found");

            if (lDto.Identifier.Contains("@") && !user.EmailConfirmed) return Unauthorized("Email not confirmed");

            if (!BCrypt.Net.BCrypt.Verify(lDto.Password, user.PasswordHash))
                return Unauthorized("Invalid Password, Please Enter it correctly");

            string token = _tokenService.CreateJWT(user);

            return Ok(new

            { message = "You're login successfully",

                user.FullName,
                token

            });
        }

        [HttpPost("forget-password")]
        public async Task<ActionResult> ForgetPassword(ForgetPassDto Dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Dto.Email);
            if (user == null) return BadRequest("User's email not found");

            string resetToken = _tokenService.GenerateEmailVerficationToken();
            user.EmailVerficationToken = resetToken;
            user.TokenExpiration = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            string resetLink = $"{Request.Scheme}://{Request.Host}/api/AuthControlloer/reset-password?token={resetToken}";
            string body = $"<p>Click the link to reset your password: <a href='{resetLink}'>Reset Password</a></p>";

            await _emailServices.SendEmailAsync(user.Email, "Reset Your Password", body);
            return Ok("Reset Password Link has been sent to your email. ");

        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailVerficationToken == dto.Token);

            if (user == null || user.TokenExpiration < DateTime.UtcNow)
                return BadRequest("Invalid or expired token");

            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest("Passwords not match. ");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.EmailVerficationToken = null;
            user.TokenExpiration = DateTime.MinValue;
            await _context.SaveChangesAsync(); // save In DB
            return Ok("Password has been reset successfully. ");
        }

    }
}

