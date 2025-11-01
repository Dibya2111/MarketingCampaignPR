using System;
using System.Collections.Generic;

namespace MarketingCampaignServer.Models.Entities;

public partial class brand
{
    public long BrandId { get; set; }

    public string? BrandName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsActive { get; set; }
}
