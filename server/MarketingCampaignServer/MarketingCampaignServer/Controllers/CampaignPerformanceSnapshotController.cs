﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
namespace MarketingCampaignServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CampaignPerformanceSnapshotController : ControllerBase
    {
        private readonly ICampaignPerformanceSnapshotService _snapshotService;

        public CampaignPerformanceSnapshotController(ICampaignPerformanceSnapshotService snapshotService)
        {
            _snapshotService = snapshotService;
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdClaim, out var id) ? id : 0;
        }

        /// <summary>
        /// Creates a performance snapshot for a given campaign.
        /// </summary>
        [HttpPost("create/{campaignId}")]
        public async Task<IActionResult> CreateSnapshot(long campaignId)
        {
            if (campaignId <= 0)
                return BadRequest(new { message = "Invalid CampaignId. It must be greater than zero." });

            long createdByUserId = GetCurrentUserId();
            if (createdByUserId == 0)
                return Unauthorized(new { message = "User identity could not be verified." });

            var result = await _snapshotService.CreateSnapshotAsync(campaignId, createdByUserId);
            if (result == null)
                return NotFound(new { message = "Campaign not found or no metrics available." });

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all snapshots for a specific campaign.
        /// </summary>
        [HttpGet("campaign/{campaignId}")]
        public async Task<IActionResult> GetSnapshotsByCampaign(long campaignId)
        {
            if (campaignId <= 0)
                return BadRequest(new { message = "Invalid CampaignId. It must be greater than zero." });

            var snapshots = await _snapshotService.GetSnapshotsByCampaignAsync(campaignId);
            if (snapshots == null)
                return NotFound(new { message = "No snapshots found for the provided campaign." });

            return Ok(snapshots);
        }
    }
}