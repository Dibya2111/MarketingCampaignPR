namespace MarketingCampaignServer.Models.Dtos
{
    public class CampaignDto
    {
        public long CampaignId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? TotalLeads { get; set; }
        public decimal? OpenRate { get; set; }
        public decimal? ConversionRate { get; set; }
        public string Status { get; set; } = "Active";
        public long? BuyerId { get; set; }
        public long? AgencyId { get; set; }
        public long? BrandId { get; set; }
    }
}
