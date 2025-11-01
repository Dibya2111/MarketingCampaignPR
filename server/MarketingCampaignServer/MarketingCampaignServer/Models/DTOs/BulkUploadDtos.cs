using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketingCampaignServer.Models.DTOs
{
    // Single lead input (what a CSV row would contain)
    public class BulkLeadRowDto
    {
        // at minimum we expect Email and Name (per your leads entity)
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public string? Phone { get; set; }

        // Optional: CampaignId (if the uploader wants to bind all rows to a campaign)
        public long? CampaignId { get; set; }

        // Optional: Segment name (if included in CSV)
        public string? Segment { get; set; }
    }

    // Request payload for upload (a file could be converted to this server-side)
    public class BulkUploadRequestDto
    {
        // rows extracted from CSV/Excel
        [Required]
        [MinLength(1, ErrorMessage = "At least one row is required for upload.")]
        public List<BulkLeadRowDto> Rows { get; set; } = new();

        // Optional: If uploader explicitly wants all rows assigned to a campaign
        public long? ForceCampaignId { get; set; }
    }

    // Response / detail for each uploaded row
    public class BulkUploadDetailDto
    {
        public long? DetailId { get; set; }
        public string? LeadEmail { get; set; }
        public string? ValidationStatus { get; set; } // "Valid" or "Invalid"
        public string? Message { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    // Summary log of the entire upload
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
