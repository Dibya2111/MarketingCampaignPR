﻿using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MarketingCampaignServer.Data;
using MarketingCampaignServer.Helpers;
using MarketingCampaignServer.Models.Dtos;
using MarketingCampaignServer.Models.Entities;
using MarketingCampaignServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketingCampaignServer.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtTokenHelper _jwtHelper;

        public AuthService(ApplicationDbContext context, JwtTokenHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        // -----------------------------
        // User Registration
        // -----------------------------
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if username already exists
            if (await _context.users.AnyAsync(u => u.Username == registerDto.Username))
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Username already exists"
                };

            // Hash password before saving
            var hashedPassword = HashPassword(registerDto.Password);

            var user = new users
            {
                Username = registerDto.Username,
                PasswordHash = hashedPassword,
                Role = "User",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            _context.users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT
            var token = _jwtHelper.GenerateJwtToken(user.UserId, user.Username, user.Role);


            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful",
                Token = token,
                Username = user.Username,
                Role = user.Role ?? "User"
            };
        }

        // -----------------------------
        // User Login
        // -----------------------------
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password"
                };

            if (user.IsDeleted == true || user.IsActive == false)
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User account is inactive or deleted"
                };

            var token = _jwtHelper.GenerateJwtToken(user.UserId, user.Username, user.Role);


            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                Username = user.Username,
                Role = user.Role ?? "User"
            };
        }

        // -----------------------------
        // Password Utilities
        // -----------------------------
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }
    }
}
