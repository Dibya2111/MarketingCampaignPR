using System;
using System.Collections.Generic;

namespace MarketingCampaignServer.Models.Entities;

public partial class users
{
    public long UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Role { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? LastModifiedUserId { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<bulkuploadlogs> bulkuploadlogsCreatedByUser { get; set; } = new List<bulkuploadlogs>();

    public virtual ICollection<bulkuploadlogs> bulkuploadlogsLastModifiedUser { get; set; } = new List<bulkuploadlogs>();

    public virtual ICollection<bulkuploadlogs> bulkuploadlogsUploadedByNavigation { get; set; } = new List<bulkuploadlogs>();

    public virtual ICollection<campaigns> campaignsCreatedByUser { get; set; } = new List<campaigns>();

    public virtual ICollection<campaigns> campaignsLastModifiedUser { get; set; } = new List<campaigns>();

    public virtual ICollection<campaignsegments> campaignsegmentsCreatedByUser { get; set; } = new List<campaignsegments>();

    public virtual ICollection<campaignsegments> campaignsegmentsLastModifiedUser { get; set; } = new List<campaignsegments>();

    public virtual ICollection<engagementmetrics> engagementmetricsCreatedByUser { get; set; } = new List<engagementmetrics>();

    public virtual ICollection<engagementmetrics> engagementmetricsLastModifiedUser { get; set; } = new List<engagementmetrics>();

    public virtual ICollection<leads> leadsCreatedByUser { get; set; } = new List<leads>();

    public virtual ICollection<leads> leadsLastModifiedUser { get; set; } = new List<leads>();
}
