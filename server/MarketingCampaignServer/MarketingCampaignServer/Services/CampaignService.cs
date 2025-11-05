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

        public async Task<CampaignDto> CreateCampaignAsync(CreateCampaignDto dto, long createdByUserId)
        {
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
                TotalLeads = 0, 
                OpenRate = 0,   
                ConversionRate = 0, 
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

            await UpdateCampaignMetricsAsync(newCampaign.CampaignId);

            var updatedCampaign = await _context.campaigns.FirstAsync(c => c.CampaignId == newCampaign.CampaignId);

            return new CampaignDto
            {
                CampaignId = updatedCampaign.CampaignId,
                CampaignName = updatedCampaign.CampaignName,
                StartDate = updatedCampaign.StartDate,
                EndDate = updatedCampaign.EndDate,
                TotalLeads = updatedCampaign.TotalLeads,
                OpenRate = updatedCampaign.OpenRate,
                ConversionRate = updatedCampaign.ConversionRate,
                BuyerId = updatedCampaign.BuyerId,
                AgencyId = updatedCampaign.AgencyId,
                BrandId = updatedCampaign.BrandId,
                Status = updatedCampaign.Status
            };
        }

        public async Task<CampaignDto?> UpdateCampaignAsync(UpdateCampaignDto dto, long modifiedByUserId)
        {
            var existing = await _context.campaigns.FirstOrDefaultAsync(c => c.CampaignId == dto.CampaignId);
            if (existing == null)
                return null;
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

            if (dto.BuyerId.HasValue && !await _context.buyers.AnyAsync(b => b.BuyerId == dto.BuyerId.Value))
                throw new ArgumentException("Invalid BuyerId provided.");
            if (dto.AgencyId.HasValue && !await _context.agencies.AnyAsync(a => a.AgencyId == dto.AgencyId.Value))
                throw new ArgumentException("Invalid AgencyId provided.");
            if (dto.BrandId.HasValue && !await _context.brands.AnyAsync(b => b.BrandId == dto.BrandId.Value))
                throw new ArgumentException("Invalid BrandId provided.");

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

        private async Task UpdateCampaignMetricsAsync(long campaignId)
        {
            var campaign = await _context.campaigns.FirstOrDefaultAsync(c => c.CampaignId == campaignId);
            if (campaign == null) return;

            var totalLeads = await _context.leads
                .Where(l => l.CampaignId == campaignId && (l.IsDeleted == false || l.IsDeleted == null))
                .CountAsync();

            var leadsWithEngagement = await _context.leads
                .Where(l => l.CampaignId == campaignId && (l.IsDeleted == false || l.IsDeleted == null))
                .Join(_context.engagementmetrics,
                      lead => lead.LeadId,
                      metric => metric.LeadId,
                      (lead, metric) => metric)
                .Where(m => m.IsDeleted == false || m.IsDeleted == null)
                .CountAsync();

            var convertedLeads = await _context.leads
                .Where(l => l.CampaignId == campaignId && (l.IsDeleted == false || l.IsDeleted == null))
                .Join(_context.engagementmetrics,
                      lead => lead.LeadId,
                      metric => metric.LeadId,
                      (lead, metric) => metric)
                .Where(m => (m.IsDeleted == false || m.IsDeleted == null) && m.Conversions > 0)
                .Select(m => m.LeadId)
                .Distinct()
                .CountAsync();

            decimal openRate = totalLeads > 0 ? (decimal)leadsWithEngagement / totalLeads * 100 : 0;
            decimal conversionRate = totalLeads > 0 ? (decimal)convertedLeads / totalLeads * 100 : 0;

            campaign.TotalLeads = totalLeads;
            campaign.OpenRate = openRate;
            campaign.ConversionRate = conversionRate;
            campaign.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task RecalculateCampaignMetricsAsync(long campaignId)
        {
            await UpdateCampaignMetricsAsync(campaignId);
        }
    }
}
