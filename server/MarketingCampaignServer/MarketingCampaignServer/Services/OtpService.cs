using MarketingCampaignServer.Data;
using MarketingCampaignServer.Helpers;
using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Models.Entities;
using MarketingCampaignServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketingCampaignServer.Services
{
    public class OtpService : IOtpService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtTokenHelper _jwtHelper;

        public OtpService(ApplicationDbContext context, JwtTokenHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<OtpResponseDto> GenerateOtpAsync(long userId)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive == true);
            if (user == null)
            {
                return new OtpResponseDto { Success = false, Message = "User not found" };
            }

            var existingOtps = await _context.otplogins
                .Where(o => o.UserId == userId && o.IsActive == true)
                .ToListAsync();

            foreach (var otp in existingOtps)
            {
                otp.IsActive = false;
            }

            var otpCode = new Random().Next(100000, 999999).ToString();

            var otpLogin = new otplogins
            {
                UserId = userId,
                OtpCode = otpCode,
                GeneratedAt = DateTime.UtcNow,
                ExpirationTime = DateTime.UtcNow.AddMinutes(1),
                IsVerified = false,
                AttemptCount = 0,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            _context.otplogins.Add(otpLogin);
            await _context.SaveChangesAsync();

            return new OtpResponseDto 
            { 
                Success = true, 
                Message = $"OTP generated successfully",
                RemainingAttempts = 3
            };
        }

        public async Task<OtpResponseDto> VerifyOtpAsync(long userId, string otpCode)
        {
            var otpLogin = await _context.otplogins
                .Where(o => o.UserId == userId && o.IsActive == true && o.IsVerified == false)
                .OrderByDescending(o => o.GeneratedAt)
                .FirstOrDefaultAsync();

            if (otpLogin == null)
            {
                return new OtpResponseDto { Success = false, Message = "No active OTP found" };
            }

            if (DateTime.UtcNow > otpLogin.ExpirationTime)
            {
                otpLogin.IsActive = false;
                await _context.SaveChangesAsync();
                return new OtpResponseDto { Success = false, Message = "OTP has expired" };
            }

            otpLogin.AttemptCount = (otpLogin.AttemptCount ?? 0) + 1;
            otpLogin.LastModifiedDate = DateTime.UtcNow;

            if (otpLogin.AttemptCount > 3)
            {
                otpLogin.IsActive = false;
                await _context.SaveChangesAsync();
                return new OtpResponseDto { Success = false, Message = "Maximum attempts exceeded" };
            }

            if (otpLogin.OtpCode != otpCode)
            {
                await _context.SaveChangesAsync();
                int remainingAttempts = 3 - (otpLogin.AttemptCount ?? 0);
                return new OtpResponseDto 
                { 
                    Success = false, 
                    Message = "Invalid OTP code",
                    RemainingAttempts = remainingAttempts
                };
            }

            otpLogin.IsVerified = true;
            otpLogin.IsActive = false;
            await _context.SaveChangesAsync();

            var user = await _context.users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return new OtpResponseDto { Success = false, Message = "User not found" };
            }

            var token = _jwtHelper.GenerateJwtToken(user.UserId, user.Username, user.Role ?? "User");

            return new OtpResponseDto 
            { 
                Success = true, 
                Message = "OTP verified successfully",
                Token = token,
                RemainingAttempts = 0
            };
        }
    }
}