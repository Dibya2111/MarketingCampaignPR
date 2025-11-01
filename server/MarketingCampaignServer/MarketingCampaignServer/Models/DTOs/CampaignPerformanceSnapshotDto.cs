using System;

namespace MarketingCampaignServer.Models.DTOs
{
    // ✅ Used for creating new snapshot entries
    public class CreateCampaignPerformanceSnapshotDto
    {
        public long CampaignId { get; set; }
    }
// ✅ Used to return snapshot details to client
    public class CampaignPerformanceSnapshotDto
        {
            public long SnapshotId { get; set; }
            public long CampaignId { get; set; }
            public int? TotalLeads { get; set; }
            public decimal? OpenRate { get; set; }
            public decimal? ConversionRate { get; set; }
            public DateTime? DateCaptured { get; set; }
        }

}
