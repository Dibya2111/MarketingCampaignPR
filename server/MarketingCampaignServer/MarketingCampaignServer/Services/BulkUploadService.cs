using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MarketingCampaignServer.Data;
using MarketingCampaignServer.Models.DTOs;
using MarketingCampaignServer.Models.Entities;
using MarketingCampaignServer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MarketingCampaignServer.Services
{
    public class BulkUploadService : IBulkUploadService
    {
        private readonly ApplicationDbContext _context;

        // Basic email regex (sufficient for validation here)
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public BulkUploadService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BulkUploadLogDto> UploadLeadsAsync(BulkUploadRequestDto request, long uploadedByUserId)
        {
            if (request == null || request.Rows == null || !request.Rows.Any())
                throw new ArgumentException("No rows provided for upload.");

            // Create log entry (initial counts 0; will update after processing)
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
            await _context.SaveChangesAsync(); // Save to get UploadId for details

            var detailsToInsert = new List<bulkuploaddetails>();
            int valid = 0;
            int invalid = 0;

            // We'll check duplicate emails in DB first (query all existing emails to speed up)
            var inputEmails = request.Rows.Select(r => r.Email.Trim().ToLower()).ToList();
            var existingEmailsInDb = await _context.leads
                .Where(l => inputEmails.Contains(l.Email.ToLower()))
                .Select(l => l.Email.ToLower())
                .ToListAsync();

            var seenInFile = new HashSet<string>(); // to detect duplicates inside uploaded file

            // Optional: If ForceCampaignId provided, verify campaign exists once
            long? forcedCampaignId = request.ForceCampaignId;
            if (forcedCampaignId.HasValue)
            {
                var campaignExists = await _context.campaigns.AnyAsync(c => c.CampaignId == forcedCampaignId.Value && (c.IsDeleted == false || c.IsDeleted == null));
                if (!campaignExists)
                {
                    // mark whole upload as invalid: update log and return
                    uploadLog.ValidRecords = 0;
                    uploadLog.InvalidRecords = request.Rows.Count;
                    uploadLog.TotalRecords = request.Rows.Count;
                    uploadLog.LastModifiedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    // Create bulkuploaddetails entries for each row marking invalid reason
                    foreach (var row in request.Rows)
                    {
                        detailsToInsert.Add(new bulkuploaddetails
                        {
                            UploadId = uploadLog.UploadId,
                            LeadEmail = row.Email,
                            ValidationStatus = "Invalid",
                            Message = $"Forced CampaignId {forcedCampaignId.Value} not found.",
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                    _context.bulkuploaddetails.AddRange(detailsToInsert);
                    await _context.SaveChangesAsync();

                    return new BulkUploadLogDto
                    {
                        UploadId = uploadLog.UploadId,
                        UploadedBy = uploadLog.UploadedBy ?? uploadedByUserId,
                        UploadedAt = uploadLog.UploadedAt ?? DateTime.UtcNow,
                        TotalRecords = uploadLog.TotalRecords ?? request.Rows.Count,
                        ValidRecords = 0,
                        InvalidRecords = request.Rows.Count
                    };
                }
            }

            // Iterate rows and validate -> either insert lead or create detail invalid entry
            foreach (var row in request.Rows)
            {
                var emailNorm = (row.Email ?? string.Empty).Trim();
                var emailLower = emailNorm.ToLower();

                string status;
                string message = string.Empty;

                // Basic required checks
                if (string.IsNullOrWhiteSpace(emailNorm) || string.IsNullOrWhiteSpace(row.Name))
                {
                    status = "Invalid";
                    message = "Missing required fields (Name and/or Email).";
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
                else if (existingEmailsInDb.Contains(emailLower))
                {
                    status = "Invalid";
                    message = "Duplicate email already exists in system.";
                }
                else
                {
                    // Optional: verify segment exists if provided; else default to 'General'
                    string? effectiveSegment = row.Segment;
                    if (!string.IsNullOrWhiteSpace(effectiveSegment))
                    {
                        var segExists = await _context.campaignsegments.AnyAsync(s => s.SegmentName == effectiveSegment && (s.IsDeleted == false || s.IsDeleted == null));
                        if (!segExists)
                        {
                            // Mark invalid but do not stop the entire upload
                            status = "Invalid";
                            message = $"Segment '{effectiveSegment}' not found.";
                        }
                        else
                        {
                            status = "Valid";
                        }
                    }
                    else
                    {
                        status = "Valid";
                    }

                    // Optional: check CampaignId existence (either per-row or forced)
                    long? finalCampaignId = row.CampaignId ?? forcedCampaignId;
                    if (status == "Valid" && finalCampaignId.HasValue)
                    {
                        var campExists = await _context.campaigns.AnyAsync(c => c.CampaignId == finalCampaignId.Value && (c.IsDeleted == false || c.IsDeleted == null));
                        if (!campExists)
                        {
                            status = "Invalid";
                            message = $"CampaignId '{finalCampaignId.Value}' not found.";
                        }
                    }
                }

                // if valid -> insert into leads
                if (status == "Valid")
                {
                    // create lead entity
                    var lead = new leads
                    {
                        Name = row.Name.Trim(),
                        Email = emailNorm,
                        Phone = string.IsNullOrWhiteSpace(row.Phone) ? null : row.Phone.Trim(),
                        CampaignId = request.ForceCampaignId ?? row.CampaignId,
                        Segment = string.IsNullOrWhiteSpace(row.Segment) ? "General" : row.Segment.Trim(),
                        CreatedByUserId = uploadedByUserId == 0 ? null : uploadedByUserId,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false
                    };

                    // Add lead
                    _context.leads.Add(lead);

                    // mark email as used so further rows in same file are duplicates
                    seenInFile.Add(emailLower);

                    // add a detail entry showing valid
                    detailsToInsert.Add(new bulkuploaddetails
                    {
                        UploadId = uploadLog.UploadId,
                        LeadEmail = emailNorm,
                        ValidationStatus = "Valid",
                        Message = "Inserted into leads.",
                        CreatedDate = DateTime.UtcNow
                    });

                    valid++;
                }
                else
                {
                    // invalid: just record the detail
                    detailsToInsert.Add(new bulkuploaddetails
                    {
                        UploadId = uploadLog.UploadId,
                        LeadEmail = row.Email,
                        ValidationStatus = "Invalid",
                        Message = message,
                        CreatedDate = DateTime.UtcNow
                    });

                    invalid++;
                }
            } // end foreach rows

            // Save leads (and details) in a transaction
            using (var tx = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Save leads already added to ChangeTracker
                    await _context.SaveChangesAsync();

                    // Insert details
                    if (detailsToInsert.Any())
                    {
                        _context.bulkuploaddetails.AddRange(detailsToInsert);
                        await _context.SaveChangesAsync();
                    }

                    // Update upload log counts
                    uploadLog.ValidRecords = valid;
                    uploadLog.InvalidRecords = invalid;
                    uploadLog.TotalRecords = request.Rows.Count;
                    uploadLog.LastModifiedDate = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    await tx.CommitAsync();
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            }

            return new BulkUploadLogDto
            {
                UploadId = uploadLog.UploadId,
                UploadedBy = uploadLog.UploadedBy ?? uploadedByUserId,
                UploadedAt = uploadLog.UploadedAt ?? DateTime.UtcNow,
                TotalRecords = uploadLog.TotalRecords ?? request.Rows.Count,
                ValidRecords = uploadLog.ValidRecords ?? valid,
                InvalidRecords = uploadLog.InvalidRecords ?? invalid
            };
        }

        public async Task<IEnumerable<BulkUploadLogDto>> GetUploadLogsAsync()
        {
            var logs = await _context.bulkuploadlogs
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

        public async Task<IEnumerable<BulkUploadDetailDto>> GetUploadDetailsAsync(long uploadId)
        {
            var details = await _context.bulkuploaddetails
                .Where(d => d.UploadId == uploadId)
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
