using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingCampaignServer.Models.DTOs;

namespace MarketingCampaignServer.Services.Interfaces
{
    public interface IBulkUploadService
    {
        Task<BulkUploadLogDto> UploadLeadsAsync(BulkUploadRequestDto request, long uploadedByUserId);
        Task<IEnumerable<BulkUploadLogDto>> GetUploadLogsAsync(long userId);

        Task<IEnumerable<BulkUploadDetailDto>> GetUploadDetailsAsync(long uploadId, long userId);
    }
}
