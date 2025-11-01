using System.ComponentModel.DataAnnotations;

namespace MarketingCampaignServer.Models.DTOs
{
    public class UpdateCampaignDto
    {
        [Required]
        public long CampaignId { get; set; }

        [Required(ErrorMessage = "Campaign name is required")]
        public string CampaignName { get; set; } = string.Empty;

        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public int? TotalLeads { get; set; }
        public decimal? OpenRate { get; set; }
        public decimal? ConversionRate { get; set; }

        public long? BuyerId { get; set; }
        public long? AgencyId { get; set; }
        public long? BrandId { get; set; }


        public string? Status { get; set; }
    }
}
