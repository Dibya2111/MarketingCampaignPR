using System;
using System.Collections.Generic;

namespace MarketingCampaignServer.Models.Entities;

public partial class campaignperformancesnapshots
{
    public long SnapshotId { get; set; }

    public long? CampaignId { get; set; }

    public DateTime? DateCaptured { get; set; }

    public int? TotalLeads { get; set; }

    public decimal? OpenRate { get; set; }

    public decimal? ConversionRate { get; set; }

    public virtual campaigns? Campaign { get; set; }
}
