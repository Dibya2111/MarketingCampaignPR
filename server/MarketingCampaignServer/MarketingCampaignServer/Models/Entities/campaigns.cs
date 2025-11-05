using System;
using System.Collections.Generic;

namespace MarketingCampaignServer.Models.Entities
{
    public partial class campaigns
    {
        public long CampaignId { get; set; }

        public string CampaignName { get; set; } = null!;

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public int? TotalLeads { get; set; }

        public decimal? OpenRate { get; set; }

        public decimal? ConversionRate { get; set; }

        public string? Status { get; set; }

        public long? BuyerId { get; set; }
        public long? AgencyId { get; set; }
        public long? BrandId { get; set; }

        public long? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? LastModifiedUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual users? CreatedByUser { get; set; }
        public virtual users? LastModifiedUser { get; set; }
        public virtual buyer? Buyer { get; set; }
        public virtual agencies? Agency { get; set; }
        public virtual brand? Brand { get; set; }

        public virtual ICollection<campaignperformancesnapshots> campaignperformancesnapshots { get; set; } = new List<campaignperformancesnapshots>();
        public virtual ICollection<leads> leads { get; set; } = new List<leads>();
        public virtual ICollection<engagementmetrics> engagementmetrics { get; set; }

    }
}
