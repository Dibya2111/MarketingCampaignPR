using MarketingCampaignServer.Models.Dtos;
using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Models.Entities;

namespace MarketingCampaignServer.Services.Interfaces
{
    public interface ICampaignService
    {
        Task<List<CampaignDto>> GetAllCampaignsAsync();
        Task<CampaignDto?> GetCampaignByIdAsync(long campaignId);
        Task<CampaignDto> CreateCampaignAsync(CreateCampaignDto dto, long createdByUserId);
        Task<CampaignDto?> UpdateCampaignAsync(UpdateCampaignDto dto, long modifiedByUserId);
        Task<bool> DeleteCampaignAsync(long campaignId);
        Task RecalculateCampaignMetricsAsync(long campaignId);
    }
}
