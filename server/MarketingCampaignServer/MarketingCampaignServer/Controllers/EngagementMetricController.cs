using System;
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
    [Authorize] // 🔒 Protect all endpoints with JWT
    public class EngagementMetricController : ControllerBase
    {
        private readonly IEngagementMetricService _metricService;

        public EngagementMetricController(IEngagementMetricService metricService)
        {
            _metricService = metricService;
        }

        // ✅ Helper method to get current logged-in user ID from JWT
        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(userIdClaim, out var id) ? id : 0;
        }

        // -------------------------------------------------------------
        // 🟢 POST: Create new engagement metric
        // -------------------------------------------------------------
        [HttpPost("create")]
        public async Task<IActionResult> CreateMetric([FromBody] CreateEngagementMetricDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            long userId = GetCurrentUserId();
            var created = await _metricService.CreateMetricAsync(dto, userId);

            if (created == null)
                return BadRequest(new { message = "Lead or Campaign not found." });

            return Ok(new
            {
                message = "Engagement metric created successfully.",
                data = created
            });
        }

        // -------------------------------------------------------------
        // 🟡 PUT: Update an existing engagement metric
        // -------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMetric(long id, [FromBody] UpdateEngagementMetricDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.MetricId = id;
            long userId = GetCurrentUserId();

            var updated = await _metricService.UpdateMetricAsync(dto, userId);
            if (updated == null)
                return NotFound(new { message = "Metric not found for update." });

            return Ok(new
            {
                message = "Engagement metric updated successfully.",
                data = updated
            });
        }

        // -------------------------------------------------------------
        // 🔴 DELETE: Hard delete an engagement metric
        // -------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMetric(long id)
        {
            var deleted = await _metricService.DeleteMetricAsync(id);
            if (!deleted)
                return NotFound(new { message = "Metric not found or already deleted." });

            return Ok(new { message = "Engagement metric deleted successfully." });
        }

        // -------------------------------------------------------------
        // 🟠 GET: Get metrics by campaign
        // -------------------------------------------------------------
        [HttpGet("campaign/{campaignId}")]
        public async Task<IActionResult> GetMetricsByCampaign(long campaignId)
        {
            var result = await _metricService.GetMetricsByCampaignAsync(campaignId);
            return Ok(result);
        }

        // -------------------------------------------------------------
        // 🔵 GET: Get metrics by lead
        // -------------------------------------------------------------
        [HttpGet("lead/{leadId}")]
        public async Task<IActionResult> GetMetricsByLead(long leadId)
        {
            var result = await _metricService.GetMetricsByLeadAsync(leadId);
            return Ok(result);
        }
    }
}
