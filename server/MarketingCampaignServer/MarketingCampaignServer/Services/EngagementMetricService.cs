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
    public class EngagementMetricService : IEngagementMetricService
    {
        private readonly ApplicationDbContext _context;

        public EngagementMetricService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create metric from counts -> calculate rates and store them
        public async Task<EngagementMetricDto?> CreateMetricAsync(CreateEngagementMetricDto dto, long createdByUserId)
        {
            var lead = await _context.leads.FirstOrDefaultAsync(l => l.LeadId == dto.LeadId);
            if (lead == null) return null;

            var campaignId = dto.CampaignId ?? lead.CampaignId;

            if (dto.EmailsSent < 0 || dto.EmailsOpened < 0 || dto.Clicks < 0 || dto.Conversions < 0)
                throw new ArgumentException("Counts must be zero or positive.");

            decimal openRate = 0m, clickRate = 0m, conversionRate = 0m;
            if (dto.EmailsSent > 0)
            {
                openRate = Math.Round((dto.EmailsOpened / (decimal)dto.EmailsSent) * 100m, 2);
                clickRate = Math.Round((dto.Clicks / (decimal)dto.EmailsSent) * 100m, 2);
                conversionRate = Math.Round((dto.Conversions / (decimal)dto.EmailsSent) * 100m, 2);
            }

            var metric = new engagementmetrics
            {
                LeadId = dto.LeadId,
                CampaignId = campaignId,
                OpenRate = openRate,
                ClickRate = clickRate,
                ConversionRate = conversionRate,
                CreatedByUserId = createdByUserId == 0 ? null : createdByUserId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            _context.engagementmetrics.Add(metric);
            await _context.SaveChangesAsync();

            if (campaignId.HasValue)
                await UpdateCampaignPerformanceAsync(campaignId.Value);

            return new EngagementMetricDto
            {
                MetricId = metric.MetricId,
                LeadId = metric.LeadId,
                CampaignId = metric.CampaignId,
                OpenRate = metric.OpenRate,
                ClickRate = metric.ClickRate,
                ConversionRate = metric.ConversionRate,
                CreatedDate = metric.CreatedDate,
                LastModifiedDate = metric.LastModifiedDate
            };
        }
        public async Task<EngagementMetricDto?> UpdateMetricAsync(UpdateEngagementMetricDto dto, long modifiedByUserId)
        {
            var metric = await _context.engagementmetrics.FirstOrDefaultAsync(m => m.MetricId == dto.MetricId);
            if (metric == null) return null;

            bool countsProvided = dto.EmailsSent.HasValue || dto.EmailsOpened.HasValue || dto.Clicks.HasValue || dto.Conversions.HasValue;

            if (countsProvided)
            {
                int emailsSent = dto.EmailsSent ?? 0;
                int emailsOpened = dto.EmailsOpened ?? 0;
                int clicks = dto.Clicks ?? 0;
                int conversions = dto.Conversions ?? 0;

                if (emailsSent < 0 || emailsOpened < 0 || clicks < 0 || conversions < 0)
                    throw new ArgumentException("Counts must be zero or positive.");

                decimal openRate = 0m, clickRate = 0m, conversionRate = 0m;
                if (emailsSent > 0)
                {
                    openRate = Math.Round((emailsOpened / (decimal)emailsSent) * 100m, 2);
                    clickRate = Math.Round((clicks / (decimal)emailsSent) * 100m, 2);
                    conversionRate = Math.Round((conversions / (decimal)emailsSent) * 100m, 2);
                }

                metric.OpenRate = openRate;
                metric.ClickRate = clickRate;
                metric.ConversionRate = conversionRate;

            }
            else
            {
                if (dto.OpenRate.HasValue) metric.OpenRate = Math.Round(dto.OpenRate.Value, 2);
                if (dto.ClickRate.HasValue) metric.ClickRate = Math.Round(dto.ClickRate.Value, 2);
                if (dto.ConversionRate.HasValue) metric.ConversionRate = Math.Round(dto.ConversionRate.Value, 2);
            }

            metric.LastModifiedUserId = modifiedByUserId == 0 ? metric.LastModifiedUserId : modifiedByUserId;
            metric.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            if (metric.CampaignId.HasValue)
                await UpdateCampaignPerformanceAsync(metric.CampaignId.Value);

            return new EngagementMetricDto
            {
                MetricId = metric.MetricId,
                LeadId = metric.LeadId,
                CampaignId = metric.CampaignId,
                OpenRate = metric.OpenRate,
                ClickRate = metric.ClickRate,
                ConversionRate = metric.ConversionRate,
                CreatedDate = metric.CreatedDate,
                LastModifiedDate = metric.LastModifiedDate
            };
        }

        public async Task<bool> DeleteMetricAsync(long metricId)
        {
            var metric = await _context.engagementmetrics.FirstOrDefaultAsync(m => m.MetricId == metricId);
            if (metric == null) return false;

            var campaignId = metric.CampaignId;

            _context.engagementmetrics.Remove(metric);
            await _context.SaveChangesAsync();

            if (campaignId.HasValue)
                await UpdateCampaignPerformanceAsync(campaignId.Value);

            return true;
        }

        public async Task<IEnumerable<EngagementMetricDto>> GetMetricsByCampaignAsync(long campaignId)
        {
            var list = await _context.engagementmetrics
                .Where(m => m.CampaignId == campaignId && (m.IsDeleted == false || m.IsDeleted == null))
                .OrderByDescending(m => m.CreatedDate)
                .ToListAsync();

            return list.Select(m => new EngagementMetricDto
            {
                MetricId = m.MetricId,
                LeadId = m.LeadId,
                CampaignId = m.CampaignId,
                OpenRate = m.OpenRate,
                ClickRate = m.ClickRate,
                ConversionRate = m.ConversionRate,
                CreatedDate = m.CreatedDate,
                LastModifiedDate = m.LastModifiedDate
            });
        }

        public async Task<IEnumerable<EngagementMetricDto>> GetMetricsByLeadAsync(long leadId)
        {
            var list = await _context.engagementmetrics
                .Where(m => m.LeadId == leadId && (m.IsDeleted == false || m.IsDeleted == null))
                .OrderByDescending(m => m.CreatedDate)
                .ToListAsync();

            return list.Select(m => new EngagementMetricDto
            {
                MetricId = m.MetricId,
                LeadId = m.LeadId,
                CampaignId = m.CampaignId,
                OpenRate = m.OpenRate,
                ClickRate = m.ClickRate,
                ConversionRate = m.ConversionRate,
                CreatedDate = m.CreatedDate,
                LastModifiedDate = m.LastModifiedDate
            });
        }

        public async Task<bool> RecalculateCampaignPerformanceAsync(long campaignId)
        {
            var campaign = await _context.campaigns.FirstOrDefaultAsync(c => c.CampaignId == campaignId);
            if (campaign == null) return false;

            await UpdateCampaignPerformanceAsync(campaignId);
            return true;
        }

        private async Task UpdateCampaignPerformanceAsync(long campaignId)
        {
            var metrics = await _context.engagementmetrics
                .Where(m => m.CampaignId == campaignId && (m.IsDeleted == false || m.IsDeleted == null))
                .ToListAsync();

            var campaign = await _context.campaigns.FirstOrDefaultAsync(c => c.CampaignId == campaignId);
            if (campaign == null) return;

            if (!metrics.Any())
            {
                campaign.OpenRate = 0m;
                campaign.ConversionRate = 0m;
                campaign.LastModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return;
            }

            var avgOpenRate = Math.Round(metrics.Average(m => m.OpenRate ?? 0m), 2);
            var avgConversionRate = Math.Round(metrics.Average(m => m.ConversionRate ?? 0m), 2);

            campaign.OpenRate = avgOpenRate;
            campaign.ConversionRate = avgConversionRate;
            campaign.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
