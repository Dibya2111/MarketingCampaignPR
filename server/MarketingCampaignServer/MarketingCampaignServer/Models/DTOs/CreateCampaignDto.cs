using System.ComponentModel.DataAnnotations;

namespace MarketingCampaignServer.Models.DTOs
{
    public class CreateCampaignDto
    {
        [Required(ErrorMessage = "Campaign name is required")]
        public string CampaignName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateOnly EndDate { get; set; }

        public int? TotalLeads { get; set; } = 0;

        public string? Status { get; set; } = "Active";

        public long? BuyerId { get; set; }
        public long? AgencyId { get; set; }
        public long? BrandId { get; set; }
    }
}
