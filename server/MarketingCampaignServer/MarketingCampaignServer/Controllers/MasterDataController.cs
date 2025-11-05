using MarketingCampaignServer.Data;
using MarketingCampaignServer.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketingCampaignServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MasterDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("buyers")]
        public async Task<IActionResult> GetBuyers()
        {
            var data = await _context.buyers
                .Where(b => b.IsActive == true)
                .Select(b => new MasterItemDto
                {
                    Id = b.BuyerId,
                    Name = b.BuyerName,
                    IsActive = b.IsActive
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("agencies")]
        public async Task<IActionResult> GetAgencies()
        {
            var data = await _context.agencies
                .Where(a => a.IsActive == true)
                .Select(a => new MasterItemDto
                {
                    Id = a.AgencyId,
                    Name = a.AgencyName,
                    IsActive = a.IsActive
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("brands")]
        public async Task<IActionResult> GetBrands()
        {
            var data = await _context.brands
                .Where(b => b.IsActive == true)
                .Select(b => new MasterItemDto
                {
                    Id = b.BrandId,
                    Name = b.BrandName,
                    IsActive = b.IsActive
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("segments")]
        public async Task<IActionResult> GetSegments()
        {
            var data = await _context.campaignsegments
                .Where(s => s.IsActive == true)
                .Select(s => new MasterItemDto
                {
                    Id = s.SegmentId,
                    Name = s.SegmentName,
                    IsActive = s.IsActive
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}
