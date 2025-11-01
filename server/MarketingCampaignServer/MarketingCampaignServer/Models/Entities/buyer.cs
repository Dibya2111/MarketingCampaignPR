using System;
using System.Collections.Generic;

namespace MarketingCampaignServer.Models.Entities;

public partial class buyer
{
    public long BuyerId { get; set; }

    public string? BuyerName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsActive { get; set; }
}
