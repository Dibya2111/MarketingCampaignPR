using MarketingCampaignServer.Data;
using MarketingCampaignServer.Helpers;
using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Models.Entities;
using MarketingCampaignServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketingCampaignServer.Services
{
    public class LeadService : ILeadService
    {
        private readonly ApplicationDbContext _context;
        private readonly SegmentAssignmentHelper _segmentHelper;

        public LeadService(ApplicationDbContext context, SegmentAssignmentHelper segmentHelper)
        {
            _context = context;
            _segmentHelper = segmentHelper;
        }

        // ✅ GET with filters and pagination
        public async Task<(List<LeadDto> Items, int TotalCount)> GetLeadsAsync(long? campaignId = null,
            string? segment = null, string? search = null, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var query = _context.leads
                .AsQueryable()
                .Where(l => l.IsDeleted == false || l.IsDeleted == null);

            if (campaignId.HasValue)
                query = query.Where(l => l.CampaignId == campaignId.Value);

            if (!string.IsNullOrWhiteSpace(segment))
                query = query.Where(l => l.Segment != null && l.Segment.Contains(segment));

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(l => l.Name.Contains(s) || l.Email.Contains(s) || (l.Phone ?? "").Contains(s));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LeadDto
                {
                    LeadId = l.LeadId,
                    Name = l.Name,
                    Email = l.Email,
                    Phone = l.Phone,
                    CampaignId = l.CampaignId,
                    Segment = l.Segment,
                    CreatedByUserId = l.CreatedByUserId,
                    CreatedDate = l.CreatedDate,
                    LastModifiedUserId = l.LastModifiedUserId,
                    LastModifiedDate = l.LastModifiedDate,
                    IsActive = l.IsActive,
                    IsDeleted = l.IsDeleted
                })
                .ToListAsync();

            return (items, total);
        }

        // ✅ GET single lead by ID
        public async Task<LeadDto?> GetLeadByIdAsync(long id)
        {
            var l = await _context.leads
                .FirstOrDefaultAsync(x => x.LeadId == id && (x.IsDeleted == false || x.IsDeleted == null));

            if (l == null) return null;

            return new LeadDto
            {
                LeadId = l.LeadId,
                Name = l.Name,
                Email = l.Email,
                Phone = l.Phone,
                CampaignId = l.CampaignId,
                Segment = l.Segment,
                CreatedByUserId = l.CreatedByUserId,
                CreatedDate = l.CreatedDate,
                LastModifiedUserId = l.LastModifiedUserId,
                LastModifiedDate = l.LastModifiedDate,
                IsActive = l.IsActive,
                IsDeleted = l.IsDeleted
            };
        }

        // ✅ CREATE Lead (auto segment assignment)
        public async Task<LeadDto> CreateLeadAsync(CreateLeadDto dto, long createdByUserId)
        {
            // Check for duplicates
            var exists = await _context.leads.AnyAsync(x => x.Email == dto.Email && (x.IsDeleted == false || x.IsDeleted == null));
            if (exists)
                throw new InvalidOperationException("A lead with the same email already exists.");

            // Validate campaign
            if (dto.CampaignId.HasValue)
            {
                var campaignExists = await _context.campaigns.AnyAsync(c => c.CampaignId == dto.CampaignId.Value && (c.IsDeleted == false || c.IsDeleted == null));
                if (!campaignExists)
                    throw new ArgumentException("Provided CampaignId does not exist.");
            }

            // ✅ Auto-assign segment
            var segmentName = await _segmentHelper.AssignSegmentAsync(dto.Email, dto.Phone);

            var entity = new leads
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                CampaignId = dto.CampaignId,
                Segment = segmentName,
                CreatedByUserId = createdByUserId == 0 ? null : createdByUserId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            _context.leads.Add(entity);
            await _context.SaveChangesAsync();

            return new LeadDto
            {
                LeadId = entity.LeadId,
                Name = entity.Name,
                Email = entity.Email,
                Phone = entity.Phone,
                CampaignId = entity.CampaignId,
                Segment = entity.Segment,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted
            };
        }

        // ✅ UPDATE Lead (reassigns segment if email/phone changes)
        public async Task<LeadDto?> UpdateLeadAsync(UpdateLeadDto dto, long modifiedByUserId)
        {
            var existing = await _context.leads.FirstOrDefaultAsync(x => x.LeadId == dto.LeadId && (x.IsDeleted == false || x.IsDeleted == null));
            if (existing == null) return null;

            // Validate campaign if provided
            if (dto.CampaignId.HasValue)
            {
                var campaignExists = await _context.campaigns.AnyAsync(c => c.CampaignId == dto.CampaignId.Value && (c.IsDeleted == false || c.IsDeleted == null));
                if (!campaignExists)
                    throw new ArgumentException("Provided CampaignId does not exist.");
            }

            // ✅ Update fields
            existing.Name = dto.Name ?? existing.Name;
            existing.Email = dto.Email ?? existing.Email;
            existing.Phone = dto.Phone ?? existing.Phone;
            existing.CampaignId = dto.CampaignId ?? existing.CampaignId;
            existing.IsActive = dto.IsActive ?? existing.IsActive;
            existing.LastModifiedUserId = modifiedByUserId == 0 ? existing.LastModifiedUserId : modifiedByUserId;
            existing.LastModifiedDate = DateTime.UtcNow;

            // ✅ Recalculate segment if email or phone was updated
            if (!string.Equals(existing.Email, dto.Email, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(existing.Phone, dto.Phone, StringComparison.OrdinalIgnoreCase))
            {
                existing.Segment = await _segmentHelper.AssignSegmentAsync(existing.Email, existing.Phone);
            }

            await _context.SaveChangesAsync();

            return new LeadDto
            {
                LeadId = existing.LeadId,
                Name = existing.Name,
                Email = existing.Email,
                Phone = existing.Phone,
                CampaignId = existing.CampaignId,
                Segment = existing.Segment,
                CreatedByUserId = existing.CreatedByUserId,
                CreatedDate = existing.CreatedDate,
                LastModifiedUserId = existing.LastModifiedUserId,
                LastModifiedDate = existing.LastModifiedDate,
                IsActive = existing.IsActive,
                IsDeleted = existing.IsDeleted
            };
        }

        // ✅ Hard delete
        public async Task<bool> DeleteLeadAsync(long id)
        {
            var existing = await _context.leads.FirstOrDefaultAsync(x => x.LeadId == id);
            if (existing == null) return false;

            _context.leads.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
