using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingCampaignServer.Models.DTOs;

namespace MarketingCampaignServer.Services.Interfaces
{
    public interface IEngagementMetricService
    {
        Task<EngagementMetricDto?> CreateMetricAsync(CreateEngagementMetricDto dto, long createdByUserId);
        Task<EngagementMetricDto?> UpdateMetricAsync(UpdateEngagementMetricDto dto, long modifiedByUserId);
        Task<bool> DeleteMetricAsync(long metricId);
        Task<IEnumerable<EngagementMetricDto>> GetMetricsByCampaignAsync(long campaignId);
        Task<IEnumerable<EngagementMetricDto>> GetMetricsByLeadAsync(long leadId);

        // implement this helper so controllers or jobs can force a recalculation
        Task<bool> RecalculateCampaignPerformanceAsync(long campaignId);
    }
}
