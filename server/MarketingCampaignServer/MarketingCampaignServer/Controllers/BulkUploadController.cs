using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Services.Interfaces;
using System.Security.Claims;

namespace MarketingCampaignServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BulkUploadController : ControllerBase
    {
        private readonly IBulkUploadService _bulkService;

        public BulkUploadController(IBulkUploadService bulkService)
        {
            _bulkService = bulkService;
        }

        // helper to get user id from JWT
        private long GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(claim, out var id) ? id : 0;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromBody] BulkUploadRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "User not identified" });

            var result = await _bulkService.UploadLeadsAsync(dto, userId);
            return Ok(new { message = "Upload processed", log = result });
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs()
        {
            var userId = GetCurrentUserId();
            var logs = await _bulkService.GetUploadLogsAsync(userId);
            return Ok(logs);
        }

        [HttpGet("details/{uploadId}")]
        public async Task<IActionResult> GetDetails(long uploadId)
        {
            if (uploadId <= 0)
                return BadRequest(new { message = "Invalid uploadId" });

            var userId = GetCurrentUserId();
            var details = await _bulkService.GetUploadDetailsAsync(uploadId, userId);
            return Ok(details);
        }
    }
}
