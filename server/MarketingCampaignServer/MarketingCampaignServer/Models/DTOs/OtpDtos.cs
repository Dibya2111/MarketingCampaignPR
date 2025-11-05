using System.ComponentModel.DataAnnotations;

namespace MarketingCampaignServer.Models.DTOs
{
    public class GenerateOtpDto
    {
        [Required]
        public long UserId { get; set; }
    }

    public class VerifyOtpDto
    {
        [Required]
        public long UserId { get; set; }
        
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string OtpCode { get; set; } = null!;
    }

    public class OtpResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string? Token { get; set; }
        public int RemainingAttempts { get; set; }
    }
}