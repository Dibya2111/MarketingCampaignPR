using System;
using System.Collections.Generic;

namespace MarketingCampaignServer.Models.Entities;

public partial class agencies
{
    public long AgencyId { get; set; }

    public string? AgencyName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsActive { get; set; }
}
