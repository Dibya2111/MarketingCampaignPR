using System.ComponentModel.DataAnnotations;

namespace MarketingCampaignServer.Models.DTOs
{
    public class UpdateLeadDto
    {
        [Required]
        public long LeadId { get; set; }

        [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters.")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(150, ErrorMessage = "Email must not exceed 150 characters.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(30, ErrorMessage = "Phone must not exceed 30 characters.")]
        public string? Phone { get; set; }

        public long? CampaignId { get; set; }

        [StringLength(100, ErrorMessage = "Segment must not exceed 100 characters.")]
        public string? Segment { get; set; }
        public bool? IsActive { get; set; }
    }
}
