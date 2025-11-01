using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingCampaignServer.Data;
using MarketingCampaignServer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketingCampaignServer.Helpers
{
    /// <summary>
    /// Helper for assigning segments to leads based on predefined campaignsegments rules.
    /// </summary>
    public class SegmentAssignmentHelper
    {
        private readonly ApplicationDbContext _context;

        public SegmentAssignmentHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Automatically determines the correct segment name for a given lead.
        /// </summary>
        public async Task<string> AssignSegmentAsync(string email, string? phone)
        {
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
                return "General"; // fallback

            var segments = await _context.campaignsegments
                .Where(s => s.IsActive == true && s.IsDeleted == false)
                .ToListAsync();

            // ✅ Email domain-based rules
            string domain = email?.Split('@').LastOrDefault()?.ToLower() ?? "";

            if (domain.EndsWith("company.com"))
                return segments.FirstOrDefault(s => s.SegmentName == "Corporate Leads")?.SegmentName ?? "Corporate Leads";

            if (domain.EndsWith("edu.org"))
                return segments.FirstOrDefault(s => s.SegmentName == "Student/Academic")?.SegmentName ?? "Student/Academic";

            if (domain.EndsWith("gmail.com") || domain.EndsWith("yahoo.com"))
                return segments.FirstOrDefault(s => s.SegmentName == "General Public")?.SegmentName ?? "General Public";

            // ✅ Phone-based rules
            if (!string.IsNullOrWhiteSpace(phone))
            {
                if (phone.StartsWith("+1"))
                    return segments.FirstOrDefault(s => s.SegmentName == "US Leads")?.SegmentName ?? "US Leads";

                if (phone.StartsWith("+91"))
                    return segments.FirstOrDefault(s => s.SegmentName == "India Leads")?.SegmentName ?? "India Leads";
            }

            // ✅ Default fallback
            return segments.FirstOrDefault(s => s.SegmentName == "General")?.SegmentName ?? "General";
        }
    }
}
