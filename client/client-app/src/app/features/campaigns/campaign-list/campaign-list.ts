import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CampaignService } from '../../../core/services/campaign.service';
import { MasterDataService } from '../../../core/services/master-data.service';
import { Campaign } from '../../../core/models/campaign.models';
import { MasterItem } from '../../../core/models/master-data.models';

@Component({
  selector: 'app-campaign-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './campaign-list.html',
  styleUrls: ['./campaign-list.css']
})
export class CampaignListComponent implements OnInit {
  campaigns: Campaign[] = [];
  filteredCampaigns: Campaign[] = [];
  paginatedCampaigns: Campaign[] = [];
  loading = false;
  error = '';
  
  // Pagination
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;
  
  // Sorting
  sortColumn = 'campaignName';
  sortDirection: 'asc' | 'desc' = 'asc';
  
  // Filters
  filters = {
    campaignName: '',
    startDate: '',
    endDate: '',
    status: '',
    agencies: [] as number[],
    buyers: [] as number[],
    brands: [] as number[]
  };
  
  // Master data
  agencies: MasterItem[] = [];
  buyers: MasterItem[] = [];
  brands: MasterItem[] = [];

  constructor(
    private campaignService: CampaignService,
    private masterDataService: MasterDataService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadMasterData();
    this.loadCampaigns();
  }

  loadMasterData(): void {
    this.masterDataService.getAgencies().subscribe(data => this.agencies = data);
    this.masterDataService.getBuyers().subscribe(data => this.buyers = data);
    this.masterDataService.getBrands().subscribe(data => this.brands = data);
  }

  loadCampaigns(): void {
    this.loading = true;
    this.error = '';
    
    this.campaignService.getAllCampaigns().subscribe({
      next: (campaigns) => {
        this.campaigns = campaigns;
        this.applyFilters();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load campaigns';
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredCampaigns = this.campaigns.filter(campaign => {
      return (!this.filters.campaignName || campaign.campaignName.toLowerCase().includes(this.filters.campaignName.toLowerCase())) &&
             (!this.filters.startDate || campaign.startDate >= this.filters.startDate) &&
             (!this.filters.endDate || campaign.endDate <= this.filters.endDate) &&
             (!this.filters.status || campaign.status === this.filters.status) &&
             (this.filters.agencies.length === 0 || (campaign.agencyId && this.filters.agencies.includes(campaign.agencyId))) &&
             (this.filters.buyers.length === 0 || (campaign.buyerId && this.filters.buyers.includes(campaign.buyerId))) &&
             (this.filters.brands.length === 0 || (campaign.brandId && this.filters.brands.includes(campaign.brandId)));
    });
    
    this.sortCampaigns();
    this.updatePagination();
  }

  sortCampaigns(): void {
    this.filteredCampaigns.sort((a, b) => {
      const aVal = (a as any)[this.sortColumn];
      const bVal = (b as any)[this.sortColumn];
      const result = aVal < bVal ? -1 : aVal > bVal ? 1 : 0;
      return this.sortDirection === 'asc' ? result : -result;
    });
  }

  updatePagination(): void {
    this.totalPages = Math.ceil(this.filteredCampaigns.length / this.pageSize);
    const start = (this.currentPage - 1) * this.pageSize;
    this.paginatedCampaigns = this.filteredCampaigns.slice(start, start + this.pageSize);
  }

  onSort(column: string): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }
    this.applyFilters();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.updatePagination();
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.applyFilters();
  }

  toggleMultiSelect(array: number[], value: number): void {
    const index = array.indexOf(value);
    if (index > -1) {
      array.splice(index, 1);
    } else {
      array.push(value);
    }
    this.onFilterChange();
  }

  createCampaign(): void {
    this.router.navigate(['/campaigns/new']);
  }

  editCampaign(id: number): void {
    this.router.navigate(['/campaigns/edit', id]);
  }

  deleteCampaign(campaign: Campaign): void {
    if (confirm(`Are you sure you want to delete "${campaign.campaignName}"?`)) {
      this.campaignService.deleteCampaign(campaign.campaignId).subscribe({
        next: () => this.loadCampaigns(),
        error: (err) => this.error = 'Failed to delete campaign'
      });
    }
  }

  getSortIcon(column: string): string {
    if (this.sortColumn !== column) return '↑↓';
    return this.sortDirection === 'asc' ? '↑' : '↓';
  }

  getAgencyName(agencyId?: number): string {
    if (!agencyId) return '-';
    const agency = this.agencies.find(a => a.id === agencyId);
    return agency ? agency.name : '-';
  }

  getBuyerName(buyerId?: number): string {
    if (!buyerId) return '-';
    const buyer = this.buyers.find(b => b.id === buyerId);
    return buyer ? buyer.name : '-';
  }

  getBrandName(brandId?: number): string {
    if (!brandId) return '-';
    const brand = this.brands.find(b => b.id === brandId);
    return brand ? brand.name : '-';
  }

  Math = Math;

  viewAnalytics(campaignId: number): void {
    this.router.navigate(['/analytics/campaign', campaignId]);
  }

  exportCampaigns(): void {
    const headers = ['Campaign Name', 'Start Date', 'End Date', 'Total Leads', 'Open Rate (%)', 'Conversion Rate (%)', 'Agency', 'Buyer', 'Brand', 'Status'];
    const csvData = this.paginatedCampaigns.map(campaign => [
      campaign.campaignName,
      campaign.startDate,
      campaign.endDate,
      campaign.totalLeads || 0,
      campaign.openRate || 0,
      campaign.conversionRate || 0,
      this.getAgencyName(campaign.agencyId),
      this.getBuyerName(campaign.buyerId),
      this.getBrandName(campaign.brandId),
      campaign.status
    ]);

    const csvContent = [headers, ...csvData]
      .map(row => row.map(field => `"${field}"`).join(','))
      .join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `campaigns_export_${new Date().toISOString().split('T')[0]}.csv`;
    link.click();
    window.URL.revokeObjectURL(url);
  }
}

