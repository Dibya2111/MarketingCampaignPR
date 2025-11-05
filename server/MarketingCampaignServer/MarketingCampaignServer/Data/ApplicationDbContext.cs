using System;
using System.Collections.Generic;
using MarketingCampaignServer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace MarketingCampaignServer.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<agencies> agencies { get; set; }

    public virtual DbSet<brand> brands { get; set; }

    public virtual DbSet<bulkuploaddetails> bulkuploaddetails { get; set; }

    public virtual DbSet<bulkuploadlogs> bulkuploadlogs { get; set; }

    public virtual DbSet<buyer> buyers { get; set; }

    public virtual DbSet<campaigns> campaigns { get; set; }

    public virtual DbSet<campaignperformancesnapshots> campaignperformancesnapshots { get; set; }

    public virtual DbSet<campaignsegments> campaignsegments { get; set; }

    public virtual DbSet<engagementmetrics> engagementmetrics { get; set; }

    public virtual DbSet<leads> leads { get; set; }

    public virtual DbSet<users> users { get; set; }

    public virtual DbSet<otplogins> otplogins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<agencies>(entity =>
        {
            entity.HasKey(e => e.AgencyId).HasName("PRIMARY");

            entity.Property(e => e.AgencyName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PRIMARY");

            entity.Property(e => e.BrandName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<bulkuploaddetails>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PRIMARY");

            entity.HasIndex(e => e.UploadId, "UploadId");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.LeadEmail).HasMaxLength(150);
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.ValidationStatus).HasMaxLength(50);

            entity.HasOne(d => d.Upload).WithMany(p => p.bulkuploaddetails)
                .HasForeignKey(d => d.UploadId)
                .HasConstraintName("bulkuploaddetails_ibfk_1");
        });

        modelBuilder.Entity<bulkuploadlogs>(entity =>
        {
            entity.HasKey(e => e.UploadId).HasName("PRIMARY");

            entity.HasIndex(e => e.CreatedByUserId, "CreatedByUserId");

            entity.HasIndex(e => e.LastModifiedUserId, "LastModifiedUserId");

            entity.HasIndex(e => e.UploadedBy, "UploadedBy");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.InvalidRecords).HasDefaultValueSql("'0'");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("'0'");
            entity.Property(e => e.LastModifiedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.TotalRecords).HasDefaultValueSql("'0'");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.ValidRecords).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.bulkuploadlogsCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("bulkuploadlogs_ibfk_2");

            entity.HasOne(d => d.LastModifiedUser).WithMany(p => p.bulkuploadlogsLastModifiedUser)
                .HasForeignKey(d => d.LastModifiedUserId)
                .HasConstraintName("bulkuploadlogs_ibfk_3");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.bulkuploadlogsUploadedByNavigation)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("bulkuploadlogs_ibfk_1");
        });

        modelBuilder.Entity<buyer>(entity =>
        {
            entity.HasKey(e => e.BuyerId).HasName("PRIMARY");

            entity.Property(e => e.BuyerName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
        });

        modelBuilder.Entity<campaigns>(entity =>
        {
            entity.HasKey(e => e.CampaignId).HasName("PRIMARY");

            entity.HasIndex(e => e.CreatedByUserId, "CreatedByUserId");

            entity.HasIndex(e => e.LastModifiedUserId, "LastModifiedUserId");

            entity.Property(e => e.CampaignName).HasMaxLength(100);
            entity.Property(e => e.ConversionRate)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("'0'");
            entity.Property(e => e.LastModifiedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.OpenRate)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Active'");
            entity.Property(e => e.TotalLeads).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.campaignsCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("campaigns_ibfk_1");

            entity.HasOne(d => d.LastModifiedUser).WithMany(p => p.campaignsLastModifiedUser)
                .HasForeignKey(d => d.LastModifiedUserId)
                .HasConstraintName("campaigns_ibfk_2");
        });

        modelBuilder.Entity<campaignperformancesnapshots>(entity =>
        {
            entity.HasKey(e => e.SnapshotId).HasName("PRIMARY");

            entity.HasIndex(e => e.CampaignId, "CampaignId");

            entity.Property(e => e.ConversionRate).HasPrecision(5, 2);
            entity.Property(e => e.DateCaptured)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.OpenRate).HasPrecision(5, 2);

            entity.HasOne(d => d.Campaign).WithMany(p => p.campaignperformancesnapshots)
                .HasForeignKey(d => d.CampaignId)
                .HasConstraintName("campaignperformancesnapshots_ibfk_1");
        });

        modelBuilder.Entity<campaignsegments>(entity =>
        {
            entity.HasKey(e => e.SegmentId).HasName("PRIMARY");

            entity.HasIndex(e => e.CreatedByUserId, "CreatedByUserId");

            entity.HasIndex(e => e.LastModifiedUserId, "LastModifiedUserId");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("'0'");
            entity.Property(e => e.LastModifiedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.SegmentName).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.campaignsegmentsCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("campaignsegments_ibfk_1");

            entity.HasOne(d => d.LastModifiedUser).WithMany(p => p.campaignsegmentsLastModifiedUser)
                .HasForeignKey(d => d.LastModifiedUserId)
                .HasConstraintName("campaignsegments_ibfk_2");
        });

        modelBuilder.Entity<engagementmetrics>(entity =>
        {
            entity.HasKey(e => e.MetricId).HasName("PRIMARY");

            entity.HasIndex(e => e.CreatedByUserId, "CreatedByUserId");

            entity.HasIndex(e => e.LastModifiedUserId, "LastModifiedUserId");

            entity.HasIndex(e => e.LeadId, "LeadId");

            entity.HasIndex(e => e.CampaignId, "fk_campaign");

            entity.Property(e => e.ClickRate)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.Clicks).HasDefaultValueSql("'0'");
            entity.Property(e => e.ConversionRate)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.Conversions).HasDefaultValueSql("'0'");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.EmailsOpened).HasDefaultValueSql("'0'");
            entity.Property(e => e.EmailsSent).HasDefaultValueSql("'0'");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("'0'");
            entity.Property(e => e.LastModifiedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.OpenRate)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("'0.00'");

            entity.HasOne(d => d.Campaign).WithMany(p => p.engagementmetrics)
                .HasForeignKey(d => d.CampaignId)
                .HasConstraintName("fk_campaign");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.engagementmetricsCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("engagementmetrics_ibfk_2");

            entity.HasOne(d => d.LastModifiedUser).WithMany(p => p.engagementmetricsLastModifiedUser)
                .HasForeignKey(d => d.LastModifiedUserId)
                .HasConstraintName("engagementmetrics_ibfk_3");

            entity.HasOne(d => d.Lead).WithMany(p => p.engagementmetrics)
                .HasForeignKey(d => d.LeadId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("engagementmetrics_ibfk_1");
        });

        modelBuilder.Entity<leads>(entity =>
        {
            entity.HasKey(e => e.LeadId).HasName("PRIMARY");

            entity.HasIndex(e => e.CampaignId, "CampaignId");

            entity.HasIndex(e => e.CreatedByUserId, "CreatedByUserId");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.HasIndex(e => e.LastModifiedUserId, "LastModifiedUserId");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("'0'");
            entity.Property(e => e.LastModifiedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Segment)
                .HasMaxLength(50)
                .HasDefaultValueSql("'General'");

            entity.HasOne(d => d.Campaign).WithMany(p => p.leads)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("leads_ibfk_1");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.leadsCreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("leads_ibfk_2");

            entity.HasOne(d => d.LastModifiedUser).WithMany(p => p.leadsLastModifiedUser)
                .HasForeignKey(d => d.LastModifiedUserId)
                .HasConstraintName("leads_ibfk_3");
        });

        modelBuilder.Entity<users>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("'0'");
            entity.Property(e => e.LastModifiedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValueSql("'User'");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
