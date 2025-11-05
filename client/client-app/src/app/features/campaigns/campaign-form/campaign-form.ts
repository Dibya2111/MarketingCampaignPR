import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CampaignService } from '../../../core/services/campaign.service';
import { MasterDataService } from '../../../core/services/master-data.service';
import { Campaign } from '../../../core/models/campaign.models';
import { MasterItem } from '../../../core/models/master-data.models';

@Component({
  selector: 'app-campaign-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './campaign-form.html',
  styleUrls: ['./campaign-form.css']
})
export class CampaignFormComponent implements OnInit {
  campaignForm: FormGroup;
  loading = false;
  error = '';
  success = '';
  isEditMode = false;
  campaignId?: number;
  
  // Master data
  agencies: MasterItem[] = [];
  buyers: MasterItem[] = [];
  brands: MasterItem[] = [];

  constructor(
    private fb: FormBuilder,
    private campaignService: CampaignService,
    private masterDataService: MasterDataService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.campaignForm = this.fb.group({
      campaignName: ['', [Validators.required, Validators.minLength(3)]],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      status: ['Active', Validators.required],
      agencyId: [''],
      buyerId: [''],
      brandId: ['']
    });
  }

  ngOnInit(): void {
    this.loadMasterData();
    this.campaignId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.campaignId) {
      this.isEditMode = true;
      this.loadCampaign();
    }
  }

  loadMasterData(): void {
    this.masterDataService.getAgencies().subscribe(data => this.agencies = data);
    this.masterDataService.getBuyers().subscribe(data => this.buyers = data);
    this.masterDataService.getBrands().subscribe(data => this.brands = data);
  }

  loadCampaign(): void {
    if (!this.campaignId) return;
    
    this.loading = true;
    this.campaignService.getCampaignById(this.campaignId).subscribe({
      next: (campaign) => {
        this.campaignForm.patchValue({
          campaignName: campaign.campaignName,
          startDate: campaign.startDate,
          endDate: campaign.endDate,
          status: campaign.status,
          agencyId: campaign.agencyId,
          buyerId: campaign.buyerId,
          brandId: campaign.brandId
        });
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load campaign';
        this.loading = false;
        console.error('Error loading campaign:', err);
      }
    });
  }

  onSubmit(): void {
    if (this.campaignForm.valid) {
      this.loading = true;
      this.error = '';
      this.success = '';

      const formData = this.campaignForm.value;
      
      if (this.isEditMode && this.campaignId) {
        const updateData = {
          campaignId: this.campaignId,
          ...formData
        };
        
        this.campaignService.updateCampaign(updateData).subscribe({
          next: (response) => {
            this.success = 'Campaign updated successfully!';
            this.loading = false;
            setTimeout(() => this.router.navigate(['/campaigns']), 1500);
          },
          error: (err) => {
            this.error = err.error?.message || 'Failed to update campaign';
            this.loading = false;
          }
        });
      } else {
        this.campaignService.createCampaign(formData).subscribe({
          next: (campaign) => {
            this.success = 'Campaign created successfully!';
            this.loading = false;
            setTimeout(() => this.router.navigate(['/campaigns']), 1500);
          },
          error: (err) => {
            this.error = err.error?.message || 'Failed to create campaign';
            this.loading = false;
          }
        });
      }
    }
  }

  cancel(): void {
    this.router.navigate(['/campaigns']);
  }
}

