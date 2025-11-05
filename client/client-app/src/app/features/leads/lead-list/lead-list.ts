import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LeadService } from '../../../core/services/lead.service';
import { CampaignService } from '../../../core/services/campaign.service';
import { MasterDataService } from '../../../core/services/master-data.service';
import { Lead } from '../../../core/models/lead.models';
import { Campaign } from '../../../core/models/campaign.models';
import { MasterItem } from '../../../core/models/master-data.models';
import { LeadAnalyticsComponent } from '../../analytics/lead-analytics';

@Component({
  selector: 'app-lead-list',
  standalone: true,
  imports: [CommonModule, FormsModule, LeadAnalyticsComponent],
  templateUrl: './lead-list.html',
  styleUrls: ['./lead-list.css']
})
export class LeadListComponent implements OnInit {
  leads: Lead[] = [];
  campaigns: Campaign[] = [];
  segments: MasterItem[] = [];
  loading = false;
  error = '';
  
  // Pagination
  currentPage = 1;
  pageSize = 20;
  totalLeads = 0;
  totalPages = 0;
  
  // Filters
  filters = {
    campaignId: '',
    segment: '',
    search: ''
  };
  
  // Multiple search
  multipleSearch = '';
  allLeads: Lead[] = []; // Store all leads for filtering

  // Analytics modal
  showAnalyticsModal = false;
  selectedLeadId = 0;

  constructor(
    private leadService: LeadService,
    private campaignService: CampaignService,
    private masterDataService: MasterDataService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCampaigns();
    this.loadSegments();
    this.loadLeads();
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

  loadLeads(): void {
    this.loading = true;
    this.error = '';
    
    // If multiple search is active, use client-side filtering
    if (this.multipleSearch.trim()) {
      this.loadAllLeadsForMultipleSearch();
      return;
    }
    
    const campaignId = this.filters.campaignId ? Number(this.filters.campaignId) : undefined;
    const segment = this.filters.segment || undefined;
    const search = this.filters.search || undefined;
    
    this.leadService.getLeads(campaignId, segment, search, this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        this.leads = response.items;
        this.totalLeads = response.total;
        this.totalPages = Math.ceil(this.totalLeads / this.pageSize);
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load leads';
        this.loading = false;
        console.error('Error loading leads:', err);
      }
    });
  }
  
  loadAllLeadsForMultipleSearch(): void {
    const campaignId = this.filters.campaignId ? Number(this.filters.campaignId) : undefined;
    const segment = this.filters.segment || undefined;
    
    // Load all leads without pagination for multiple search
    this.leadService.getLeads(campaignId, segment, undefined, 1, 10000).subscribe({
      next: (response) => {
        this.allLeads = response.items;
        this.applyMultipleSearch();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load leads';
        this.loading = false;
        console.error('Error loading leads:', err);
      }
    });
  }
  
  applyMultipleSearch(): void {
    if (!this.multipleSearch.trim()) {
      this.leads = [];
      this.totalLeads = 0;
      this.totalPages = 0;
      return;
    }
    
    const searchTerms = this.multipleSearch
      .split('\n')
      .map(term => term.trim().toLowerCase())
      .filter(term => term.length > 0);
    
    const filteredLeads = this.allLeads.filter(lead => {
      const leadName = lead.name.toLowerCase();
      const leadEmail = lead.email.toLowerCase();
      
      return searchTerms.some(term => 
        leadName.includes(term) || leadEmail.includes(term)
      );
    });
    
    this.totalLeads = filteredLeads.length;
    this.totalPages = Math.ceil(this.totalLeads / this.pageSize);
    
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.leads = filteredLeads.slice(startIndex, endIndex);
  }

  onFilterChange(): void {
    if (this.filters.search) {
      this.multipleSearch = '';
    }
    this.currentPage = 1;
    this.loadLeads();
  }
  
  onMultipleSearchChange(): void {
    if (this.multipleSearch) {
      this.filters.search = '';
    }
    this.currentPage = 1;
    this.loadLeads();
  }
  
  getSearchResultsCount(): string {
    if (!this.multipleSearch.trim()) return '';
    const searchTerms = this.multipleSearch.split('\n').filter(term => term.trim().length > 0);
    return `Searching for ${searchTerms.length} term(s)`;
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadLeads();
  }

  createLead(): void {
    this.router.navigate(['/leads/new']);
  }

  editLead(id: number): void {
    this.router.navigate(['/leads/edit', id]);
  }

  deleteLead(lead: Lead): void {
    if (confirm(`Are you sure you want to delete lead "${lead.name}"?`)) {
      this.leadService.deleteLead(lead.leadId).subscribe({
        next: () => this.loadLeads(),
        error: (err) => {
          this.error = 'Failed to delete lead';
          console.error('Error deleting lead:', err);
        }
      });
    }
  }

  getCampaignName(campaignId?: number): string {
    if (!campaignId) return '-';
    const campaign = this.campaigns.find(c => c.campaignId === campaignId);
    return campaign ? campaign.campaignName : '-';
  }

  Math = Math;

  viewAnalytics(leadId: number): void {
    this.selectedLeadId = leadId;
    this.showAnalyticsModal = true;
  }

  exportLeads(): void {
    const headers = ['Name', 'Email', 'Phone', 'Campaign', 'Segment', 'Status', 'Created Date'];
    const csvData = this.leads.map(lead => [
      lead.name,
      lead.email,
      lead.phone || '',
      this.getCampaignName(lead.campaignId),
      lead.segment || 'General',
      lead.isActive ? 'Active' : 'Inactive',
      lead.createdDate ? new Date(lead.createdDate).toLocaleDateString() : ''
    ]);

    const csvContent = [headers, ...csvData]
      .map(row => row.map(field => `"${field}"`).join(','))
      .join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `leads_export_${new Date().toISOString().split('T')[0]}.csv`;
    link.click();
    window.URL.revokeObjectURL(url);
  }
}

