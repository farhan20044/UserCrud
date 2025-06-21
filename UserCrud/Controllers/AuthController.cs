using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserCrud.Helpers;
using UserCrud.Models;
using UserCrud.Models.Dto;
using UserCrud.Services;
using UserCrud.Services.Interfaces;

namespace UserCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<ApplicationUser> userManager, IJwtService jwtService, IEmailService emailService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                //email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(
                    nameof(ConfirmEmailGet),
                    "Auth",
                    new { userId = user.Id, token },
                    protocol: HttpContext.Request.Scheme);
                await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink);
                return Ok(ErrorMessages.RegistrationSuccessfull);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailGet([FromQuery] string userId, [FromQuery] string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(ErrorMessages.InvalidUsers);
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(ErrorMessages.ExpiredToken);
            }

            return Ok(new { 
                message = ErrorMessages.EmailConfirmed,
                userId = userId,
                token = token
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(ErrorMessages.InvalidEmailPass);
            }
            if (!user.EmailConfirmed)
            {
                return Unauthorized(ErrorMessages.ConfirmEmail);
            }
            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return Unauthorized(ErrorMessages.InvalidEmailPass);
            }
            var token = _jwtService.GenerateJwtToken(user);
            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.PhoneNumber
                }
            });
        }
    }
} 