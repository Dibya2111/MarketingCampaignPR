import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { LeadService } from '../../../core/services/lead.service';
import { CampaignService } from '../../../core/services/campaign.service';
import { MasterDataService } from '../../../core/services/master-data.service';
import { Lead } from '../../../core/models/lead.models';
import { Campaign } from '../../../core/models/campaign.models';
import { MasterItem } from '../../../core/models/master-data.models';

@Component({
  selector: 'app-lead-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './lead-form.html',
  styleUrls: ['./lead-form.css']
})
export class LeadFormComponent implements OnInit {
  leadForm: FormGroup;
  loading = false;
  error = '';
  success = '';
  isEditMode = false;
  leadId?: number;
  previewedSegment = '';
  
  campaigns: Campaign[] = [];
  segments: MasterItem[] = [];

  constructor(
    private fb: FormBuilder,
    private leadService: LeadService,
    private campaignService: CampaignService,
    private masterDataService: MasterDataService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.leadForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(150)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
      phone: ['', [Validators.maxLength(30)]],
      campaignId: [''],
      segment: [''],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadCampaigns();
    this.loadSegments();
    this.leadId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.leadId) {
      this.isEditMode = true;
      this.loadLead();
    }
    
    this.leadForm.get('email')?.valueChanges.subscribe(() => this.previewSegment());
    this.leadForm.get('phone')?.valueChanges.subscribe(() => this.previewSegment());
  }

  loadCampaigns(): void {
    this.campaignService.getAllCampaigns().subscribe({
      next: (campaigns) => this.campaigns = campaigns,
      error: (err) => console.error('Error loading campaigns:', err)
    });
  }

  loadSegments(): void {
    this.masterDataService.getSegments().subscribe({
      next: (segments) => this.segments = segments,
      error: (err) => console.error('Error loading segments:', err)
    });
  }

  loadLead(): void {
    if (!this.leadId) return;
    
    this.loading = true;
    this.leadService.getLeadById(this.leadId).subscribe({
      next: (lead) => {
        this.leadForm.patchValue({
          name: lead.name,
          email: lead.email,
          phone: lead.phone,
          campaignId: lead.campaignId || '',
          segment: lead.segment || '',
          isActive: lead.isActive
        });
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load lead';
        this.loading = false;
        console.error('Error loading lead:', err);
      }
    });
  }

  previewSegment(): void {
    const email = this.leadForm.get('email')?.value;
    const phone = this.leadForm.get('phone')?.value;
    
    if (email && !this.isEditMode) {
      this.leadService.previewSegment(email, phone).subscribe({
        next: (response) => {
          this.previewedSegment = response.segment;
          if (!this.leadForm.get('segment')?.value) {
            this.leadForm.patchValue({ segment: response.segment });
          }
        },
        error: (err) => console.error('Error previewing segment:', err)
      });
    }
  }

  onSubmit(): void {
    if (this.leadForm.valid) {
      this.loading = true;
      this.error = '';
      this.success = '';

      const formData = this.leadForm.value;
      
      if (this.isEditMode && this.leadId) {
        const updateData = {
          leadId: this.leadId,
          ...formData
        };
        
        this.leadService.updateLead(updateData).subscribe({
          next: (response) => {
            this.success = response.message || 'Lead updated successfully!';
            this.loading = false;
            setTimeout(() => this.router.navigate(['/leads']), 1500);
          },
          error: (err) => {
            this.error = err.error?.message || 'Failed to update lead';
            this.loading = false;
          }
        });
      } else {
        this.leadService.createLead(formData).subscribe({
          next: (response) => {
            this.success = response.message || 'Lead created successfully!';
            this.loading = false;
            setTimeout(() => this.router.navigate(['/leads']), 1500);
          },
          error: (err) => {
            this.error = err.error?.message || 'Failed to create lead';
            this.loading = false;
          }
        });
      }
    }
  }

  cancel(): void {
    this.router.navigate(['/leads']);
  }
}

