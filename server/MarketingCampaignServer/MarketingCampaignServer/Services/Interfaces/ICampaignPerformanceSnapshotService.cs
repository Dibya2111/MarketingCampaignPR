using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingCampaignServer.Models.DTOs;
namespace MarketingCampaignServer.Services.Interfaces
{
    public interface ICampaignPerformanceSnapshotService
    {
        Task<CampaignPerformanceSnapshotDto?> CreateSnapshotAsync(long campaignId, long createdByUserId);

        Task<IEnumerable<CampaignPerformanceSnapshotDto>> GetSnapshotsByCampaignAsync(long campaignId);

        Task<IEnumerable<CampaignPerformanceSnapshotDto>> GetAllSnapshotsAsync();
    }

}