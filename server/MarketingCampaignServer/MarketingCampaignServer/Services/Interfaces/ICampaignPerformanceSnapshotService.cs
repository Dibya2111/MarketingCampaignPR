using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingCampaignServer.Models.DTOs;
namespace MarketingCampaignServer.Services.Interfaces
{
    public interface ICampaignPerformanceSnapshotService
    {
        // Create a new performance snapshot
        Task<CampaignPerformanceSnapshotDto?> CreateSnapshotAsync(long campaignId, long createdByUserId);

        // Get all snapshots for a campaign
        Task<IEnumerable<CampaignPerformanceSnapshotDto>> GetSnapshotsByCampaignAsync(long campaignId);
    }

}