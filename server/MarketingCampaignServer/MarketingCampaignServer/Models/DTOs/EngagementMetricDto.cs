using System;

namespace MarketingCampaignServer.Models.DTOs
{
    // DTO for creating a new engagement metric using raw counts
    public class CreateEngagementMetricDto
    {
        public long LeadId { get; set; }
        public long? CampaignId { get; set; }

        // raw counts (required)
        public int EmailsSent { get; set; }
        public int EmailsOpened { get; set; }
        public int Clicks { get; set; }
        public int Conversions { get; set; }
    }

    // DTO for updating an existing engagement metric.
    // Accepts either counts (preferred) or direct rates (optional).
    public class UpdateEngagementMetricDto
    {
        public long MetricId { get; set; }

        // optional counts
        public int? EmailsSent { get; set; }
        public int? EmailsOpened { get; set; }
        public int? Clicks { get; set; }
        public int? Conversions { get; set; }

        // optional direct rates (percentages). Service will prefer counts if provided.
        public decimal? OpenRate { get; set; }
        public decimal? ClickRate { get; set; }
        public decimal? ConversionRate { get; set; }
    }

    // Returned DTO with calculated rates
    public class EngagementMetricDto
    {
        public long MetricId { get; set; }
        public long? LeadId { get; set; }
        public long? CampaignId { get; set; }

        // calculated rates (percent values, 0..100)
        public decimal? OpenRate { get; set; }
        public decimal? ClickRate { get; set; }
        public decimal? ConversionRate { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
