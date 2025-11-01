using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingCampaignServer.Models.DTOs;

namespace MarketingCampaignServer.Services.Interfaces
{
    public interface IBulkUploadService
    {
        // Upload list of rows; returns the created log summary DTO
        Task<BulkUploadLogDto> UploadLeadsAsync(BulkUploadRequestDto request, long uploadedByUserId);

        // Get all upload logs (summary)
        Task<IEnumerable<BulkUploadLogDto>> GetUploadLogsAsync();

        // Get details for a specific upload
        Task<IEnumerable<BulkUploadDetailDto>> GetUploadDetailsAsync(long uploadId);
    }
}
