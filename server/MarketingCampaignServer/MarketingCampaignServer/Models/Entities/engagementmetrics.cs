using System;
using System.Collections.Generic;

namespace MarketingCampaignServer.Models.Entities
{
    public partial class engagementmetrics
    {
        public long MetricId { get; set; }

        public long? LeadId { get; set; }
        public int EmailsSent { get; set; }
        public int EmailsOpened { get; set; }
        public int Clicks { get; set; }
        public int Conversions { get; set; }

        public long? CampaignId { get; set; }
        public decimal? OpenRate { get; set; }
        public decimal? ClickRate { get; set; }
        public decimal? ConversionRate { get; set; }

        public long? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }

        public long? LastModifiedUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual campaigns? Campaign { get; set; }

        public virtual users? CreatedByUser { get; set; }
        public virtual users? LastModifiedUser { get; set; }
        public virtual leads? Lead { get; set; }
    }
}
