using MarketingCampaignServer.Models.DTOs;

namespace MarketingCampaignServer.Services.Interfaces
{
    public interface ILeadService
    {
        Task<(List<LeadDto> Items, int TotalCount)> GetLeadsAsync(
            long? campaignId = null,
            string? segment = null,
            string? search = null,
            int page = 1,
            int pageSize = 20,
            long? userId = null);

        Task<LeadDto?> GetLeadByIdAsync(long id, long userId);
        Task<LeadDto> CreateLeadAsync(CreateLeadDto dto, long createdByUserId);
        Task<LeadDto?> UpdateLeadAsync(UpdateLeadDto dto, long modifiedByUserId);
        Task<bool> DeleteLeadAsync(long id, long userId);
    }
}
