namespace MarketingCampaignServer.Models.DTOs
{
    public class LeadDto
    {
        public long LeadId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public long? CampaignId { get; set; }

        public string? Segment { get; set; }

        public long? CreatedByUserId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public long? LastModifiedUserId { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
