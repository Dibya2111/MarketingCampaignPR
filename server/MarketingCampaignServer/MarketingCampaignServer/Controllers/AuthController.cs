using System.Threading.Tasks;
using MarketingCampaignServer.Models.Dtos;
using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketingCampaignServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;

        public AuthController(IAuthService authService, IOtpService otpService)
        {
            _authService = authService;
            _otpService = otpService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Username and password are required" });

            var result = await _authService.LoginAsync(dto);
            if (!result.Success)
                return Unauthorized(result);

            //otp generation instead of token
            var otpResult = await _otpService.GenerateOtpAsync(result.UserId);
            if (!otpResult.Success)
                return BadRequest(otpResult);

            return Ok(new { 
                success = true, 
                message = "Credentials verified. OTP sent.",
                userId = result.UserId,
                otpMessage = otpResult.Message,
                requiresOtp = true
            });
        }
    }
}
