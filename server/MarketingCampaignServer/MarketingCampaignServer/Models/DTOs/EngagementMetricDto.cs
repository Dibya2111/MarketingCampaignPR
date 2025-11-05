using System;

namespace MarketingCampaignServer.Models.DTOs
{
    public class CreateEngagementMetricDto
    {
        public long LeadId { get; set; }
        public long? CampaignId { get; set; }

        public int EmailsSent { get; set; }
        public int EmailsOpened { get; set; }
        public int Clicks { get; set; }
        public int Conversions { get; set; }
    }
    public class UpdateEngagementMetricDto
    {
        public long MetricId { get; set; }

        public int? EmailsSent { get; set; }
        public int? EmailsOpened { get; set; }
        public int? Clicks { get; set; }
        public int? Conversions { get; set; }
        public decimal? OpenRate { get; set; }
        public decimal? ClickRate { get; set; }
        public decimal? ConversionRate { get; set; }
    }
    public class EngagementMetricDto
    {
        public long MetricId { get; set; }
        public long? LeadId { get; set; }
        public long? CampaignId { get; set; }
        public decimal? OpenRate { get; set; }
        public decimal? ClickRate { get; set; }
        public decimal? ConversionRate { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
