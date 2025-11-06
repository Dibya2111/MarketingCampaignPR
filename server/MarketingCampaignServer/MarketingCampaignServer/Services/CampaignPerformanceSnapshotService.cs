using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingCampaignServer.Data;
using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Models.Entities;
using MarketingCampaignServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace MarketingCampaignServer.Services
{
    public class CampaignPerformanceSnapshotService : ICampaignPerformanceSnapshotService
    {
        private readonly ApplicationDbContext _context;

        public CampaignPerformanceSnapshotService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CampaignPerformanceSnapshotDto?> CreateSnapshotAsync(long campaignId, long createdByUserId)
        {
            var campaign = await _context.campaigns
                .FirstOrDefaultAsync(c => c.CampaignId == campaignId && c.IsDeleted == false);

            if (campaign == null)
                return null;

            var metrics = await _context.engagementmetrics
                .Where(m => m.CampaignId == campaignId && m.IsDeleted == false)
                .ToListAsync();

            decimal avgOpenRate = 0, avgConversionRate = 0;
            int totalLeads = 0;

            if (metrics.Any())
            {
                avgOpenRate = metrics.Average(m => m.OpenRate ?? 0);
                avgConversionRate = metrics.Average(m => m.ConversionRate ?? 0);
                totalLeads = metrics.Count;
            }

            var snapshot = new campaignperformancesnapshots
            {
                CampaignId = campaignId,
                DateCaptured = DateTime.UtcNow,
                TotalLeads = totalLeads,
                OpenRate = avgOpenRate,
                ConversionRate = avgConversionRate
            };

            _context.campaignperformancesnapshots.Add(snapshot);

            campaign.OpenRate = avgOpenRate;
            campaign.ConversionRate = avgConversionRate;
            campaign.TotalLeads = totalLeads;
            campaign.LastModifiedUserId = createdByUserId;
            campaign.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new CampaignPerformanceSnapshotDto
            {
                SnapshotId = snapshot.SnapshotId,
                CampaignId = snapshot.CampaignId ?? 0,
                TotalLeads = snapshot.TotalLeads,
                OpenRate = snapshot.OpenRate,
                ConversionRate = snapshot.ConversionRate,
                DateCaptured = snapshot.DateCaptured
            };
        }

        public async Task<IEnumerable<CampaignPerformanceSnapshotDto>> GetSnapshotsByCampaignAsync(long campaignId, long userId)
        {
            var snapshots = await _context.campaignperformancesnapshots
                .Where(s => s.CampaignId == campaignId)
                .Join(_context.campaigns, s => s.CampaignId, c => c.CampaignId, (s, c) => new { s, c })
                .Where(x => x.c.CreatedByUserId == userId)
                .Select(x => x.s)
                .OrderByDescending(s => s.DateCaptured)
                .ToListAsync();

            return snapshots.Select(s => new CampaignPerformanceSnapshotDto
            {
                SnapshotId = s.SnapshotId,
                CampaignId = s.CampaignId ?? 0,
                TotalLeads = s.TotalLeads,
                OpenRate = s.OpenRate,
                ConversionRate = s.ConversionRate,
                DateCaptured = s.DateCaptured
            });
        }

        public async Task<IEnumerable<CampaignPerformanceSnapshotDto>> GetAllSnapshotsAsync(long userId)
        {
            var snapshots = await _context.campaignperformancesnapshots
                .Join(_context.campaigns, s => s.CampaignId, c => c.CampaignId, (s, c) => new { s, c })
                .Where(x => x.c.CreatedByUserId == userId)
                .Select(x => x.s)
                .OrderByDescending(s => s.DateCaptured)
                .ToListAsync();

            return snapshots.Select(s => new CampaignPerformanceSnapshotDto
            {
                SnapshotId = s.SnapshotId,
                CampaignId = s.CampaignId ?? 0,
                TotalLeads = s.TotalLeads,
                OpenRate = s.OpenRate,
                ConversionRate = s.ConversionRate,
                DateCaptured = s.DateCaptured
            });
        }
    }
}