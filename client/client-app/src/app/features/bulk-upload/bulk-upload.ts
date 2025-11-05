import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { BulkUploadService } from '../../core/services/bulk-upload.service';
import { CampaignService } from '../../core/services/campaign.service';
import { Campaign } from '../../core/models/campaign.models';
import { BulkUploadLog, BulkUploadDetail, BulkLeadRow } from '../../core/models/bulk-upload.models';
import * as Papa from 'papaparse';

@Component({
  selector: 'app-bulk-upload',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './bulk-upload.html',
  styleUrls: ['./bulk-upload.css']
})
export class BulkUploadComponent implements OnInit {
  uploadForm: FormGroup;
  campaigns: Campaign[] = [];
  uploadLogs: BulkUploadLog[] = [];
  selectedFile: File | null = null;
  loading = false;
  error = '';
  success = '';
  
  // Preview data
  previewData: BulkLeadRow[] = [];
  previewCurrentPage = 1;
  previewPageSize = 15;
  previewTotalPages = 0;
  
  showDetailsModal = false;
  selectedUploadDetails: BulkUploadDetail[] = [];
  selectedUploadId = 0;

  constructor(
    private fb: FormBuilder,
    private bulkUploadService: BulkUploadService,
    private campaignService: CampaignService
  ) {
    this.uploadForm = this.fb.group({
      forceCampaignId: ['']
    });
  }

  ngOnInit(): void {
    this.loadCampaigns();
    this.loadUploadLogs();
  }

  loadCampaigns(): void {
    this.campaignService.getAllCampaigns().subscribe({
      next: (campaigns) => this.campaigns = campaigns,
      error: (err) => console.error('Error loading campaigns:', err)
    });
  }

  loadUploadLogs(): void {
    this.bulkUploadService.getUploadLogs().subscribe({
      next: (logs) => this.uploadLogs = logs,
      error: (err) => console.error('Error loading upload logs:', err)
    });
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file && file.type === 'text/csv') {
      this.selectedFile = file;
      this.error = '';
      this.parseAndPreviewCSV(file);
    } else {
      this.error = 'Only CSV files are supported. Please select a .csv file or download the template.';
      this.selectedFile = null;
      this.clearPreview();
    }
  }
  
  parseAndPreviewCSV(file: File): void {
    Papa.parse(file, {
      header: true,
      skipEmptyLines: true,
      complete: (result) => {
        try {
          const rows = this.mapCSVData(result.data);
          this.previewData = rows;
          this.previewCurrentPage = 1;
          this.previewTotalPages = Math.ceil(rows.length / this.previewPageSize);
          
          if (rows.length === 0) {
            this.error = 'CSV file is empty or invalid. Expected columns: Name, Email, Phone (optional), CampaignId (optional)';
          }
        } catch (error) {
          this.error = 'Error parsing CSV file';
          this.clearPreview();
        }
      },
      error: (error) => {
        this.error = 'Error reading CSV file';
        this.clearPreview();
      }
    });
  }

  downloadTemplate(): void {
    const csvContent = 'Name,Email,Phone,CampaignId\nRakesh Jha,rakesh.jhs@gmail.com,+919853192292,1\nJemimiah Rodrigues,jemi.rodrigues@yahoo.com,,2';
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'bulk_upload_template.csv';
    link.click();
    window.URL.revokeObjectURL(url);
  }

  onUpload(): void {
    if (!this.selectedFile || this.previewData.length === 0) {
      this.error = 'Please select a CSV file and review the preview';
      return;
    }

    const validLeads = this.previewData.filter(lead => this.isValidLead(lead));
    if (validLeads.length === 0) {
      this.error = 'No valid leads found in the CSV file';
      return;
    }

    this.loading = true;
    this.error = '';
    this.success = '';

    const request = {
      rows: validLeads,
      forceCampaignId: this.uploadForm.value.forceCampaignId || undefined
    };

    this.bulkUploadService.uploadLeads(request).subscribe({
      next: (response) => {
        this.success = `Upload completed Successfully!!`;
        this.loading = false;
        this.selectedFile = null;
        this.uploadForm.reset();
        this.clearPreview();
        this.loadUploadLogs();
      },
      error: (err) => {
        this.error = err.error?.message || err.message || 'Upload failed';
        this.loading = false;
      }
    });
  }

  mapCSVData(data: any[]): BulkLeadRow[] {
    return data.map(row => ({
      name: row.Name || row.name || '',
      email: row.Email || row.email || '',
      phone: row.Phone || row.phone || undefined,
      campaignId: row.CampaignId || row.campaignId ? parseInt(row.CampaignId || row.campaignId) : undefined
    })).filter(row => row.name && row.email);
  }

  viewDetails(uploadId: number): void {
    this.selectedUploadId = uploadId;
    this.bulkUploadService.getUploadDetails(uploadId).subscribe({
      next: (details) => {
        this.selectedUploadDetails = details;
        this.showDetailsModal = true;
      },
      error: (err) => console.error('Error loading upload details:', err)
    });
  }

  closeModal(): void {
    this.showDetailsModal = false;
    this.selectedUploadDetails = [];
  }
  
  // Preview methods
  getDisplayedLeads(): BulkLeadRow[] {
    const startIndex = (this.previewCurrentPage - 1) * this.previewPageSize;
    const endIndex = startIndex + this.previewPageSize;
    return this.previewData.slice(startIndex, endIndex);
  }
  
  onPreviewPageChange(page: number): void {
    this.previewCurrentPage = page;
  }
  
  isValidLead(lead: BulkLeadRow): boolean {
    return !!(lead.name && lead.email && this.isValidEmail(lead.email));
  }
  
  isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }
  
  getValidLeadsCount(): number {
    return this.previewData.filter(lead => this.isValidLead(lead)).length;
  }
  
  getInvalidLeadsCount(): number {
    return this.previewData.filter(lead => !this.isValidLead(lead)).length;
  }
  
  clearPreview(): void {
    this.previewData = [];
    this.previewCurrentPage = 1;
    this.previewTotalPages = 0;
  }
}

