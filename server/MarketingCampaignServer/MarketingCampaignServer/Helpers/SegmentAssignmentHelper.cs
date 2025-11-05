using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingCampaignServer.Data;
using MarketingCampaignServer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketingCampaignServer.Helpers
{
    public class SegmentAssignmentHelper
    {
        private readonly ApplicationDbContext _context;

        public SegmentAssignmentHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> AssignSegmentAsync(string email, string? phone = null, campaigns? campaign = null)
        {
            var segments = await _context.campaignsegments
                .Where(s => s.IsActive == true && s.IsDeleted == false)
                .ToListAsync();

            if (campaign != null && !string.IsNullOrWhiteSpace(campaign.CampaignName))
            {
                var name = campaign.CampaignName.ToLower();

                if (name.Contains("summer") || name.Contains("monsoon") || name.Contains("festival") || name.Contains("holiday"))
                    return segments.FirstOrDefault(s => s.SegmentName == "Seasonal")?.SegmentName ?? "Seasonal";

                if (name.Contains("corporate") || name.Contains("enterprise") || name.Contains("b2b"))
                    return segments.FirstOrDefault(s => s.SegmentName == "Corporate")?.SegmentName ?? "Corporate";

                if (name.Contains("launch") || name.Contains("beta") || (name.Contains("new") && name.Contains("product")))
                    return segments.FirstOrDefault(s => s.SegmentName == "Early Adopters")?.SegmentName ?? "Early Adopters";
            }

            string domain = email?.Split('@').LastOrDefault()?.ToLower() ?? "";

            if (domain.EndsWith("company.com"))
                return segments.FirstOrDefault(s => s.SegmentName == "Corporate Leads")?.SegmentName ?? "Corporate Leads";

            if (domain.EndsWith("edu.org"))
                return segments.FirstOrDefault(s => s.SegmentName == "Student/Academic")?.SegmentName ?? "Student/Academic";

            if (domain.EndsWith("gmail.com") || domain.EndsWith("yahoo.com"))
                return segments.FirstOrDefault(s => s.SegmentName == "General Public")?.SegmentName ?? "General Public";

            if (!string.IsNullOrWhiteSpace(phone))
            {
                if (phone.StartsWith("+1"))
                    return segments.FirstOrDefault(s => s.SegmentName == "US Leads")?.SegmentName ?? "US Leads";

                if (phone.StartsWith("+91"))
                    return segments.FirstOrDefault(s => s.SegmentName == "India Leads")?.SegmentName ?? "India Leads";
            }

            return segments.FirstOrDefault(s => s.SegmentName == "General")?.SegmentName ?? "General";
        }
    }
}
