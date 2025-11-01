using MarketingCampaignServer.Data;
using MarketingCampaignServer.Models.Dtos;
using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Models.Entities;
using MarketingCampaignServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketingCampaignServer.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ApplicationDbContext _context;

        public CampaignService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL Campaigns
        public async Task<List<CampaignDto>> GetAllCampaignsAsync()
        {
            var campaigns = await _context.campaigns
                .Where(c => c.IsDeleted == false || c.IsDeleted == null)
                .Select(c => new CampaignDto
                {
                    CampaignId = c.CampaignId,
                    CampaignName = c.CampaignName,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    TotalLeads = c.TotalLeads,
                    OpenRate = c.OpenRate,
                    ConversionRate = c.ConversionRate,
                    Status = c.Status,
                    BuyerId = c.BuyerId,
                    AgencyId = c.AgencyId,
                    BrandId = c.BrandId
                })
                .ToListAsync();

            return campaigns;
        }

        // ✅ GET BY ID
        public async Task<CampaignDto?> GetCampaignByIdAsync(long campaignId)
        {
            var c = await _context.campaigns
                .FirstOrDefaultAsync(x => x.CampaignId == campaignId && (x.IsDeleted == false || x.IsDeleted == null));

            if (c == null)
                return null;

            return new CampaignDto
            {
                CampaignId = c.CampaignId,
                CampaignName = c.CampaignName,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                TotalLeads = c.TotalLeads,
                OpenRate = c.OpenRate,
                ConversionRate = c.ConversionRate,
                Status = c.Status,
                BuyerId = c.BuyerId,
                AgencyId = c.AgencyId,
                BrandId = c.BrandId
            };
        }

        // ✅ CREATE Campaign
        public async Task<CampaignDto> CreateCampaignAsync(CreateCampaignDto dto, long createdByUserId)
        {
            // Validate foreign keys
            if (dto.BuyerId.HasValue && !await _context.buyers.AnyAsync(b => b.BuyerId == dto.BuyerId.Value))
                throw new ArgumentException("Invalid BuyerId provided.");

            if (dto.AgencyId.HasValue && !await _context.agencies.AnyAsync(a => a.AgencyId == dto.AgencyId.Value))
                throw new ArgumentException("Invalid AgencyId provided.");

            if (dto.BrandId.HasValue && !await _context.brands.AnyAsync(b => b.BrandId == dto.BrandId.Value))
                throw new ArgumentException("Invalid BrandId provided.");

            var newCampaign = new campaigns
            {
                CampaignName = dto.CampaignName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalLeads = dto.TotalLeads ?? 0,
                BuyerId = dto.BuyerId,
                AgencyId = dto.AgencyId,
                BrandId = dto.BrandId,
                Status = dto.Status ?? "Active",
                CreatedByUserId = createdByUserId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            _context.campaigns.Add(newCampaign);
            await _context.SaveChangesAsync();

            return new CampaignDto
            {
                CampaignId = newCampaign.CampaignId,
                CampaignName = newCampaign.CampaignName,
                StartDate = newCampaign.StartDate,
                EndDate = newCampaign.EndDate,
                TotalLeads = newCampaign.TotalLeads,
                BuyerId = newCampaign.BuyerId,
                AgencyId = newCampaign.AgencyId,
                BrandId = newCampaign.BrandId,
                Status = newCampaign.Status
            };
        }

        // ✅ UPDATE Campaign
        public async Task<CampaignDto?> UpdateCampaignAsync(UpdateCampaignDto dto, long modifiedByUserId)
        {
            var existing = await _context.campaigns.FirstOrDefaultAsync(c => c.CampaignId == dto.CampaignId);
            if (existing == null)
                return null;

            // Return current data if nothing new is provided
            if (dto.CampaignName == null &&
                dto.StartDate == null &&
                dto.EndDate == null &&
                dto.TotalLeads == null &&
                dto.OpenRate == null &&
                dto.ConversionRate == null &&
                dto.Status == null &&
                dto.BuyerId == null &&
                dto.AgencyId == null &&
                dto.BrandId == null)
            {
                return new CampaignDto
                {
                    CampaignId = existing.CampaignId,
                    CampaignName = existing.CampaignName,
                    StartDate = existing.StartDate,
                    EndDate = existing.EndDate,
                    TotalLeads = existing.TotalLeads,
                    OpenRate = existing.OpenRate,
                    ConversionRate = existing.ConversionRate,
                    Status = existing.Status,
                    BuyerId = existing.BuyerId,
                    AgencyId = existing.AgencyId,
                    BrandId = existing.BrandId
                };
            }

            // Validate referenced entities if new ones provided
            if (dto.BuyerId.HasValue && !await _context.buyers.AnyAsync(b => b.BuyerId == dto.BuyerId.Value))
                throw new ArgumentException("Invalid BuyerId provided.");
            if (dto.AgencyId.HasValue && !await _context.agencies.AnyAsync(a => a.AgencyId == dto.AgencyId.Value))
                throw new ArgumentException("Invalid AgencyId provided.");
            if (dto.BrandId.HasValue && !await _context.brands.AnyAsync(b => b.BrandId == dto.BrandId.Value))
                throw new ArgumentException("Invalid BrandId provided.");

            // Apply updates
            existing.CampaignName = dto.CampaignName ?? existing.CampaignName;
            existing.StartDate = dto.StartDate ?? existing.StartDate;
            existing.EndDate = dto.EndDate ?? existing.EndDate;
            existing.TotalLeads = dto.TotalLeads ?? existing.TotalLeads;
            existing.OpenRate = dto.OpenRate ?? existing.OpenRate;
            existing.ConversionRate = dto.ConversionRate ?? existing.ConversionRate;
            existing.Status = dto.Status ?? existing.Status;
            existing.BuyerId = dto.BuyerId ?? existing.BuyerId;
            existing.AgencyId = dto.AgencyId ?? existing.AgencyId;
            existing.BrandId = dto.BrandId ?? existing.BrandId;
            existing.LastModifiedUserId = modifiedByUserId;
            existing.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new CampaignDto
            {
                CampaignId = existing.CampaignId,
                CampaignName = existing.CampaignName,
                StartDate = existing.StartDate,
                EndDate = existing.EndDate,
                TotalLeads = existing.TotalLeads,
                OpenRate = existing.OpenRate,
                ConversionRate = existing.ConversionRate,
                Status = existing.Status,
                BuyerId = existing.BuyerId,
                AgencyId = existing.AgencyId,
                BrandId = existing.BrandId
            };
        }

        // ✅ DELETE Campaign (soft delete)
        public async Task<bool> DeleteCampaignAsync(long campaignId)
        {
            var existing = await _context.campaigns.FirstOrDefaultAsync(c => c.CampaignId == campaignId);
            if (existing == null)
                return false;

            existing.IsDeleted = true;
            existing.IsActive = false;
            existing.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
