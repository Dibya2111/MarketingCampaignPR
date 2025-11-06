using MarketingCampaignServer.Models.Dtos;
using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Models.Entities;

namespace MarketingCampaignServer.Services.Interfaces
{
    public interface ICampaignService
    {
        Task<List<CampaignDto>> GetAllCampaignsAsync(long userId);
        Task<CampaignDto?> GetCampaignByIdAsync(long campaignId, long userId);
        Task<CampaignDto> CreateCampaignAsync(CreateCampaignDto dto, long createdByUserId);
        Task<CampaignDto?> UpdateCampaignAsync(UpdateCampaignDto dto, long modifiedByUserId);
        Task<bool> DeleteCampaignAsync(long campaignId, long userId);
        Task RecalculateCampaignMetricsAsync(long campaignId);
    }
}
