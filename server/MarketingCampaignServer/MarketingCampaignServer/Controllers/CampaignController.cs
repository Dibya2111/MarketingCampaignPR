using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MarketingCampaignServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // requires JWT token
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCampaigns()
        {
            var campaigns = await _campaignService.GetAllCampaignsAsync();
            return Ok(campaigns);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCampaignById(long id)
        {
            var campaign = await _campaignService.GetCampaignByIdAsync(id);
            if (campaign == null)
                return NotFound(new { message = "Campaign not found" });

            return Ok(campaign);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            long createdByUserId = GetCurrentUserId();

            var created = await _campaignService.CreateCampaignAsync(dto, createdByUserId);
            return CreatedAtAction(nameof(GetCampaignById), new { id = created.CampaignId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCampaign(long id, [FromBody] UpdateCampaignDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.CampaignId = id;
            long modifiedByUserId = GetCurrentUserId();

            var updatedCampaign = await _campaignService.UpdateCampaignAsync(dto, modifiedByUserId);

            if (updatedCampaign == null)
                return NotFound(new { message = "Campaign not found with the provided ID." });

            bool isViewOnly = dto.CampaignName == null &&
                              dto.StartDate == null &&
                              dto.EndDate == null &&
                              dto.TotalLeads == null &&
                              dto.OpenRate == null &&
                              dto.ConversionRate == null &&
                              dto.Status == null;

            if (isViewOnly)
                return Ok(new
                {
                    message = "Existing campaign details retrieved successfully.",
                    data = updatedCampaign
                });

            return Ok(new
            {
                message = "Campaign updated successfully.",
                data = updatedCampaign
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaign(long id)
        {
            bool deleted = await _campaignService.DeleteCampaignAsync(id);
            if (!deleted)
                return NotFound(new { message = "Campaign not found for deletion" });

            return Ok(new { message = "Campaign deleted successfully" });
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (long.TryParse(userIdClaim, out long userId))
                return userId;

            // fallback if no valid ID is found
            return 0;
        }
    }
}
