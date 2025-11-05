using MarketingCampaignServer.Models.DTOs;

namespace MarketingCampaignServer.Services.Interfaces
{
    public interface IOtpService
    {
        Task<OtpResponseDto> GenerateOtpAsync(long userId);
        Task<OtpResponseDto> VerifyOtpAsync(long userId, string otpCode);
    }
}