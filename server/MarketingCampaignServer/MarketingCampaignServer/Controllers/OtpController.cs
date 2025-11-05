using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketingCampaignServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otpService;

        public OtpController(IOtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateOtp([FromBody] GenerateOtpDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _otpService.GenerateOtpAsync(dto.UserId);
            
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _otpService.VerifyOtpAsync(dto.UserId, dto.OtpCode);
            
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}