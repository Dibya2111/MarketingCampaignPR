using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketingCampaignServer.Models.DTOs
{ 
    public class BulkLeadRowDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public string? Phone { get; set; }

        public long? CampaignId { get; set; }

        public string? Segment { get; set; }
    }
    public class BulkUploadRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one row is required for upload.")]
        public List<BulkLeadRowDto> Rows { get; set; } = new();

        public long? ForceCampaignId { get; set; }
    }

    public class BulkUploadDetailDto
    {
        public long? DetailId { get; set; }
        public string? LeadEmail { get; set; }
        public string? ValidationStatus { get; set; } 
        public string? Message { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class BulkUploadLogDto
    {
        public long UploadId { get; set; }
        public long UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int InvalidRecords { get; set; }
    }
}
