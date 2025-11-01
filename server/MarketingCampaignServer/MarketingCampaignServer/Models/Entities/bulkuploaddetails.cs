using System;
using System.Collections.Generic;

namespace MarketingCampaignServer.Models.Entities;

public partial class bulkuploaddetails
{
    public long DetailId { get; set; }

    public long? UploadId { get; set; }

    public string? LeadEmail { get; set; }

    public string? ValidationStatus { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual bulkuploadlogs? Upload { get; set; }
}
