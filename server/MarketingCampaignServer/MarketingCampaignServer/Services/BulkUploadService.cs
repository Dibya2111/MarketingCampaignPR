using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MarketingCampaignServer.Data;
using MarketingCampaignServer.Helpers;
using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Models.Entities;
using MarketingCampaignServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketingCampaignServer.Services
{
    public class BulkUploadService : IBulkUploadService
    {
        private readonly ApplicationDbContext _context;
        private readonly SegmentAssignmentHelper _segmentHelper;
        private readonly ICampaignService _campaignService;

        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public BulkUploadService(ApplicationDbContext context, SegmentAssignmentHelper segmentHelper, ICampaignService campaignService)
        {
            _context = context;
            _segmentHelper = segmentHelper;
            _campaignService = campaignService;
        }

        public async Task<BulkUploadLogDto> UploadLeadsAsync(BulkUploadRequestDto request, long uploadedByUserId)
        {
            if (request == null || request.Rows == null || !request.Rows.Any())
                throw new ArgumentException("No rows provided for upload.");

            var uploadLog = new bulkuploadlogs
            {
                UploadedBy = uploadedByUserId == 0 ? null : uploadedByUserId,
                UploadedAt = DateTime.UtcNow,
                TotalRecords = request.Rows.Count,
                ValidRecords = 0,
                InvalidRecords = 0,
                CreatedByUserId = uploadedByUserId == 0 ? null : uploadedByUserId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            _context.bulkuploadlogs.Add(uploadLog);
            await _context.SaveChangesAsync(); 

            var detailsToInsert = new List<bulkuploaddetails>();
            var affectedCampaigns = new HashSet<long>();
            int valid = 0;
            int invalid = 0;

            var inputEmails = request.Rows.Select(r => r.Email.Trim().ToLower()).ToList();
            var existingEmails = await _context.leads
                .Where(l => inputEmails.Contains(l.Email.ToLower()))
                .Select(l => l.Email.ToLower())
                .ToListAsync();

            var seenInFile = new HashSet<string>();

            long? forcedCampaignId = request.ForceCampaignId;
            campaigns? forcedCampaign = null;

            if (forcedCampaignId.HasValue)
            {
                forcedCampaign = await _context.campaigns
                    .FirstOrDefaultAsync(c => c.CampaignId == forcedCampaignId.Value && (c.IsDeleted == false || c.IsDeleted == null));

                if (forcedCampaign == null)
                {
                    foreach (var row in request.Rows)
                    {
                        detailsToInsert.Add(new bulkuploaddetails
                        {
                            UploadId = uploadLog.UploadId,
                            LeadEmail = row.Email,
                            ValidationStatus = "Invalid",
                            Message = $"Campaign {forcedCampaignId.Value} not found.",
                            CreatedDate = DateTime.UtcNow
                        });
                    }

                    uploadLog.ValidRecords = 0;
                    uploadLog.InvalidRecords = request.Rows.Count;
                    await _context.SaveChangesAsync();
                    return new BulkUploadLogDto
                    {
                        UploadId = uploadLog.UploadId,
                        UploadedBy = uploadedByUserId,
                        UploadedAt = DateTime.UtcNow,
                        TotalRecords = request.Rows.Count,
                        ValidRecords = 0,
                        InvalidRecords = request.Rows.Count
                    };
                }
            }

            foreach (var row in request.Rows)
            {
                var emailNorm = (row.Email ?? string.Empty).Trim();
                var emailLower = emailNorm.ToLower();

                string status;
                string message = string.Empty;

                // Basic checks
                if (string.IsNullOrWhiteSpace(emailNorm) || string.IsNullOrWhiteSpace(row.Name))
                {
                    status = "Invalid";
                    message = "Missing required fields (Name or Email).";
                }
                else if (!EmailRegex.IsMatch(emailNorm))
                {
                    status = "Invalid";
                    message = "Invalid email format.";
                }
                else if (seenInFile.Contains(emailLower))
                {
                    status = "Invalid";
                    message = "Duplicate email in uploaded file.";
                }
                else if (existingEmails.Contains(emailLower))
                {
                    status = "Invalid";
                    message = "Duplicate email already exists in system.";
                }
                else
                {
                    status = "Valid";
                }

                if (status == "Valid")
                {
                    long? campaignId = row.CampaignId ?? forcedCampaignId;
                    campaigns? campaign = forcedCampaign;

                    if (campaignId.HasValue && campaign == null)
                    {
                        campaign = await _context.campaigns
                            .FirstOrDefaultAsync(c => c.CampaignId == campaignId.Value && (c.IsDeleted == false || c.IsDeleted == null));

                        if (campaign == null)
                        {
                            status = "Invalid";
                            message = $"Campaign {campaignId.Value} not found.";
                        }
                    }

                    if (status == "Valid")
                    {
                        string segmentName = await _segmentHelper.AssignSegmentAsync(emailNorm, row.Phone, campaign);

                        var lead = new leads
                        {
                            Name = row.Name.Trim(),
                            Email = emailNorm,
                            Phone = string.IsNullOrWhiteSpace(row.Phone) ? null : row.Phone.Trim(),
                            CampaignId = campaignId,
                            Segment = segmentName,
                            CreatedByUserId = uploadedByUserId == 0 ? null : uploadedByUserId,
                            CreatedDate = DateTime.UtcNow,
                            IsActive = true,
                            IsDeleted = false
                        };

                        _context.leads.Add(lead);

                        if (campaignId.HasValue)
                            affectedCampaigns.Add(campaignId.Value);

                        detailsToInsert.Add(new bulkuploaddetails
                        {
                            UploadId = uploadLog.UploadId,
                            LeadEmail = emailNorm,
                            ValidationStatus = "Valid",
                            Message = $"Inserted successfully into leads (Segment: {segmentName}).",
                            CreatedDate = DateTime.UtcNow
                        });

                        seenInFile.Add(emailLower);
                        valid++;
                    }
                    else
                    {
                        invalid++;
                        detailsToInsert.Add(new bulkuploaddetails
                        {
                            UploadId = uploadLog.UploadId,
                            LeadEmail = emailNorm,
                            ValidationStatus = "Invalid",
                            Message = message,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                }
                else
                {
                    invalid++;
                    detailsToInsert.Add(new bulkuploaddetails
                    {
                        UploadId = uploadLog.UploadId,
                        LeadEmail = emailNorm,
                        ValidationStatus = "Invalid",
                        Message = message,
                        CreatedDate = DateTime.UtcNow
                    });
                }
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.SaveChangesAsync();
                if (detailsToInsert.Any())
                {
                    _context.bulkuploaddetails.AddRange(detailsToInsert);
                    await _context.SaveChangesAsync();
                }

                uploadLog.ValidRecords = valid;
                uploadLog.InvalidRecords = invalid;
                uploadLog.TotalRecords = request.Rows.Count;
                uploadLog.LastModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                foreach (var campaignId in affectedCampaigns)
                {
                    await _campaignService.RecalculateCampaignMetricsAsync(campaignId);
                }
                
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            return new BulkUploadLogDto
            {
                UploadId = uploadLog.UploadId,
                UploadedBy = uploadedByUserId,
                UploadedAt = DateTime.UtcNow,
                TotalRecords = request.Rows.Count,
                ValidRecords = valid,
                InvalidRecords = invalid
            };
        }

        public async Task<IEnumerable<BulkUploadLogDto>> GetUploadLogsAsync(long userId)
        {
            var logs = await _context.bulkuploadlogs
                .Where(l => l.CreatedByUserId == userId)
                .OrderByDescending(l => l.UploadedAt)
                .ToListAsync();

            return logs.Select(l => new BulkUploadLogDto
            {
                UploadId = l.UploadId,
                UploadedBy = l.UploadedBy ?? 0,
                UploadedAt = l.UploadedAt ?? DateTime.UtcNow,
                TotalRecords = l.TotalRecords ?? 0,
                ValidRecords = l.ValidRecords ?? 0,
                InvalidRecords = l.InvalidRecords ?? 0
            });
        }

        public async Task<IEnumerable<BulkUploadDetailDto>> GetUploadDetailsAsync(long uploadId, long userId)
        {
            var details = await _context.bulkuploaddetails
                .Where(d => d.UploadId == uploadId)
                .Join(_context.bulkuploadlogs, d => d.UploadId, l => l.UploadId, (d, l) => new { d, l })
                .Where(x => x.l.CreatedByUserId == userId)
                .Select(x => x.d)
                .OrderBy(d => d.DetailId)
                .ToListAsync();

            return details.Select(d => new BulkUploadDetailDto
            {
                DetailId = d.DetailId,
                LeadEmail = d.LeadEmail,
                ValidationStatus = d.ValidationStatus,
                Message = d.Message,
                CreatedDate = d.CreatedDate
            });
        }
    }
}
