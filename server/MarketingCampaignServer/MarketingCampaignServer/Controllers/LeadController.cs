using MarketingCampaignServer.Helpers;
using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MarketingCampaignServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeadController : ControllerBase
    {
        private readonly ILeadService _leadService;
        private readonly SegmentAssignmentHelper _segmentHelper;

        public LeadController(ILeadService leadService, SegmentAssignmentHelper segmentHelper)
        {
            _leadService = leadService;
            _segmentHelper = segmentHelper;
        }
0
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] long? campaignId, [FromQuery] string? segment,
            [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var (items, total) = await _leadService.GetLeadsAsync(campaignId, segment, search, page, pageSize);
            return Ok(new { items, total });
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var lead = await _leadService.GetLeadByIdAsync(id);
            if (lead == null) return NotFound(new { message = "Lead not found." });
            return Ok(lead);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLeadDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            long createdBy = GetCurrentUserId();

            try
            {
                var created = await _leadService.CreateLeadAsync(dto, createdBy);
                return CreatedAtAction(nameof(GetById), new { id = created.LeadId }, new
                {
                    message = $"Lead created successfully. Auto-assigned segment: {created.Segment}",
                    data = created
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateLeadDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            dto.LeadId = id;
            long modifiedBy = GetCurrentUserId();

            try
            {
                var updated = await _leadService.UpdateLeadAsync(dto, modifiedBy);
                if (updated == null) return NotFound(new { message = "Lead not found." });
                return Ok(new
                {
                    message = $"Lead updated successfully. Assigned segment: {updated.Segment}",
                    data = updated
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _leadService.DeleteLeadAsync(id);
            if (!deleted) return NotFound(new { message = "Lead not found." });
            return Ok(new { message = "Lead deleted permanently." });
        }

        [HttpPost("auto-segment-preview")]
        [AllowAnonymous]
        public async Task<IActionResult> PreviewSegment([FromBody] CreateLeadDto dto)
        {
            var segment = await _segmentHelper.AssignSegmentAsync(dto.Email, dto.Phone);
            return Ok(new { segment });
        }

        private long GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(idClaim, out var id) ? id : 0;
        }
    }
}
