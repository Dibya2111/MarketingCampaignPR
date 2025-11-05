using System;
using System.Collections.Generic;

namespace MarketingCampaignServer.Models.Entities;

public partial class campaignsegments
{
    public long SegmentId { get; set; }

    public string SegmentName { get; set; } = null!;

    public string? Description { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? LastModifiedUserId { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual users? CreatedByUser { get; set; }

    public virtual users? LastModifiedUser { get; set; }
}
