import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { EngagementMetricService } from '../../core/services/engagement-metric.service';
import { LeadService } from '../../core/services/lead.service';
import { CampaignService } from '../../core/services/campaign.service';
import { Lead } from '../../core/models/lead.models';
import { Campaign } from '../../core/models/campaign.models';

@Component({
  selector: 'app-engagement-upload',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './engagement-upload.html',
  styleUrls: ['./engagement-upload.css']
})
export class EngagementUploadComponent implements OnInit {
  engagementForm: FormGroup;
  leads: Lead[] = [];
  campaigns: Campaign[] = [];
  loading = false;
  error = '';
  success = '';

  constructor(
    private fb: FormBuilder,
    private engagementService: EngagementMetricService,
    private leadService: LeadService,
    private campaignService: CampaignService
  ) {
    this.engagementForm = this.fb.group({
      leadId: [''],
      manualLeadName: [''],
      manualLeadEmail: [''],
      campaignId: ['', Validators.required],
      emailsSent: [0, [Validators.required, Validators.min(0)]],
      emailsOpened: [0, [Validators.required, Validators.min(0)]],
      clicks: [0, [Validators.required, Validators.min(0)]],
      conversions: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.loadLeads();
    this.loadCampaigns();
    this.setupLeadChangeListener();
  }

  setupLeadChangeListener(): void {
    this.engagementForm.get('leadId')?.valueChanges.subscribe(leadId => {
      if (leadId) {
        const selectedLead = this.leads.find(lead => lead.leadId == leadId);
        if (selectedLead && selectedLead.campaignId) {
          // Auto-select and disable campaign field
          this.engagementForm.patchValue({ campaignId: selectedLead.campaignId });
          this.engagementForm.get('campaignId')?.disable();
        } else {
          // Enable campaign field if lead has no campaign
          this.engagementForm.get('campaignId')?.enable();
        }
      } else {
        // Enable campaign field when no lead is selected
        this.engagementForm.get('campaignId')?.enable();
      }
    });
  }

  loadLeads(): void {
    this.leadService.getLeads(undefined, undefined, undefined, 1, 1000).subscribe({
      next: (response) => this.leads = response.items,
      error: (err) => console.error('Error loading leads:', err)
    });
  }

  loadCampaigns(): void {
    this.campaignService.getAllCampaigns().subscribe({
      next: (campaigns) => this.campaigns = campaigns,
      error: (err) => console.error('Error loading campaigns:', err)
    });
  }

  onSubmit(): void {
    const formValue = this.engagementForm.getRawValue(); // Get raw value to include disabled fields
    
    // Validate required fields
    if (!formValue.campaignId) {
      this.error = 'Please select a campaign';
      return;
    }
    
    if (!formValue.leadId && (!formValue.manualLeadName || !formValue.manualLeadEmail)) {
      this.error = 'Please select a lead or enter manual lead details';
      return;
    }

    if (this.engagementForm.valid) {
      this.loading = true;
      this.error = '';
      this.success = '';

      if (!formValue.leadId && formValue.manualLeadName && formValue.manualLeadEmail) {
        const newLead = {
          name: formValue.manualLeadName,
          email: formValue.manualLeadEmail,
          campaignId: formValue.campaignId || undefined
        };
        
        this.leadService.createLead(newLead).subscribe({
          next: (leadResponse) => {
            const metricData = {
              leadId: leadResponse.data.leadId,
              campaignId: formValue.campaignId || undefined,
              emailsSent: formValue.emailsSent,
              emailsOpened: formValue.emailsOpened,
              clicks: formValue.clicks,
              conversions: formValue.conversions
            };
            this.createEngagementMetric(metricData);
          },
          error: (err) => {
            this.error = err.error?.message || 'Failed to create lead';
            this.loading = false;
          }
        });
      } else {
        const metricData = {
          leadId: formValue.leadId,
          campaignId: formValue.campaignId || undefined,
          emailsSent: formValue.emailsSent,
          emailsOpened: formValue.emailsOpened,
          clicks: formValue.clicks,
          conversions: formValue.conversions
        };
        this.createEngagementMetric(metricData);
      }
    }
  }

  createEngagementMetric(data: any): void {
    this.engagementService.createMetric(data).subscribe({
      next: (response) => {
        this.success = 'Engagement metric created successfully!';
        this.loading = false;
        this.engagementForm.reset();
        this.loadLeads(); // Refresh leads list
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to create engagement metric';
        this.loading = false;
      }
    });
  }

  generateSampleData(leadId: number): void {
    const emailsSent = Math.floor(Math.random() * 100) + 50;
    const emailsOpened = Math.floor(emailsSent * (Math.random() * 0.4 + 0.1)); // 10-50% open rate
    const clicks = Math.floor(emailsOpened * (Math.random() * 0.3 + 0.05)); // 5-35% click rate
    const conversions = Math.floor(clicks * (Math.random() * 0.2 + 0.02)); // 2-22% conversion rate

    this.engagementForm.patchValue({
      leadId: leadId,
      emailsSent: emailsSent,
      emailsOpened: emailsOpened,
      clicks: clicks,
      conversions: conversions
    });
  }
}

