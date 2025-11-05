using System;
using System.Collections.Generic;

namespace MarketingCampaignServer.Models.Entities;

public partial class leads
{
    public long LeadId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public long? CampaignId { get; set; }

    public string? Segment { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? LastModifiedUserId { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual campaigns? Campaign { get; set; }

    public virtual users? CreatedByUser { get; set; }

    public virtual users? LastModifiedUser { get; set; }

    public virtual ICollection<engagementmetrics> engagementmetrics { get; set; } = new List<engagementmetrics>();
}
