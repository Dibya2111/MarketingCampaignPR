using System;
using System.Collections.Generic;

namespace MarketingCampaignServer.Models.Entities;

public partial class bulkuploadlogs
{
    public long UploadId { get; set; }

    public long? UploadedBy { get; set; }

    public DateTime? UploadedAt { get; set; }

    public int? TotalRecords { get; set; }

    public int? ValidRecords { get; set; }

    public int? InvalidRecords { get; set; }

    public long? CreatedByUserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? LastModifiedUserId { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual users? CreatedByUser { get; set; }

    public virtual users? LastModifiedUser { get; set; }

    public virtual users? UploadedByNavigation { get; set; }

    public virtual ICollection<bulkuploaddetails> bulkuploaddetails { get; set; } = new List<bulkuploaddetails>();
}
