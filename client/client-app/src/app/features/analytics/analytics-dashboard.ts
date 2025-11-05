import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CampaignService } from '../../core/services/campaign.service';
import { CampaignPerformanceService } from '../../core/services/campaign-performance.service';
import { LeadService } from '../../core/services/lead.service';
import { Campaign } from '../../core/models/campaign.models';
import { CampaignPerformanceSnapshot } from '../../core/models/campaign-performance.models';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartType } from 'chart.js';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-analytics-dashboard',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './analytics-dashboard.html',
  styleUrls: ['./analytics-dashboard.css']
})
export class AnalyticsDashboardComponent implements OnInit {
  campaigns: Campaign[] = [];
  campaignMetrics: any[] = [];
  loading = false;
  error = '';
  
  showModal = false;
  segmentLoading = false;
  segmentData: any[] = [];

  public barChartData: ChartConfiguration['data'] = {
    datasets: [
      { data: [], label: 'Open Rate (%)', backgroundColor: '#3498db' },
      { data: [], label: 'Conversion Rate (%)', backgroundColor: '#e74c3c' }
    ],
    labels: []
  };
  public barChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    scales: { y: { beginAtZero: true, max: 100 } }
  };
  public barChartType: ChartType = 'bar';
  
  public pieChartData: ChartConfiguration['data'] = {
    datasets: [{
      data: [],
      backgroundColor: ['#3498db', '#e74c3c', '#f39c12', '#2ecc71', '#9b59b6']
    }],
    labels: []
  };
  public pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: { position: 'bottom' }
    }
  };
  public pieChartType: ChartType = 'pie';

  constructor(
    private campaignService: CampaignService,
    private performanceService: CampaignPerformanceService,
    private leadService: LeadService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAnalytics();
  }

  loadAnalytics(): void {
    this.loading = true;
    this.campaignService.getAllCampaigns().subscribe({
      next: (campaigns) => {
        this.campaigns = campaigns;
        this.loadCampaignMetrics();
      },
      error: (err) => {
        this.error = 'Failed to load campaigns';
        this.loading = false;
      }
    });
  }

  loadCampaignMetrics(): void {
    this.performanceService.getAllSnapshots().subscribe({
      next: (allSnapshots) => {
        this.campaignMetrics = this.campaigns.map(campaign => {
          const campaignSnapshots = allSnapshots.filter(s => s.campaignId === campaign.campaignId);
          const latestSnapshot = campaignSnapshots[0];
          return {
            campaign,
            totalLeads: latestSnapshot?.totalLeads || 0,
            openRate: latestSnapshot?.openRate || 0,
            conversionRate: latestSnapshot?.conversionRate || 0,
            lastUpdated: latestSnapshot?.dateCaptured
          };
        });
        this.updateChart();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load campaign metrics';
        this.loading = false;
      }
    });
  }

  updateChart(): void {
    const labels = this.campaignMetrics.map(m => m.campaign.campaignName);
    const openRates = this.campaignMetrics.map(m => m.openRate);
    const conversionRates = this.campaignMetrics.map(m => m.conversionRate);

    this.barChartData = {
      datasets: [
        { data: openRates, label: 'Open Rate (%)', backgroundColor: '#3498db' },
        { data: conversionRates, label: 'Conversion Rate (%)', backgroundColor: '#e74c3c' }
      ],
      labels: labels
    };
  }

  viewCampaignDetails(campaignId: number): void {
    this.router.navigate(['/analytics/campaign', campaignId]);
  }

  exportCampaignAnalytics(): void {
    const headers = ['Campaign Name', 'Total Leads', 'Open Rate (%)', 'Conversion Rate (%)', 'Last Updated'];
    const csvData = this.campaignMetrics.map(metric => [
      metric.campaign.campaignName,
      metric.totalLeads,
      metric.openRate,
      metric.conversionRate,
      metric.lastUpdated ? new Date(metric.lastUpdated).toLocaleDateString() : ''
    ]);

    const csvContent = [headers, ...csvData]
      .map(row => row.map(field => `"${field}"`).join(','))
      .join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `campaign_analytics_${new Date().toISOString().split('T')[0]}.csv`;
    link.click();
    window.URL.revokeObjectURL(url);
  }
  
  showSegmentChart(): void {
    this.showModal = true;
    this.loadSegmentData();
  }
  
  closeModal(): void {
    this.showModal = false;
  }
  
  loadSegmentData(): void {
    this.segmentLoading = true;
    this.leadService.getLeads(undefined, undefined, undefined, 1, 1000).subscribe({
      next: (response) => {
        const segmentCounts: { [key: string]: number } = {};
        
        response.items.forEach(lead => {
          const segment = lead.segment || 'General';
          segmentCounts[segment] = (segmentCounts[segment] || 0) + 1;
        });
        
        this.segmentData = Object.entries(segmentCounts).map(([segment, count]) => ({
          segment,
          count,
          percentage: Math.round((count / response.items.length) * 100)
        }));
        
        this.updatePieChart();
        this.segmentLoading = false;
      },
      error: (err) => {
        console.error('Error loading segment data:', err);
        this.segmentLoading = false;
      }
    });
  }
  
  updatePieChart(): void {
    const labels = this.segmentData.map(s => `${s.segment}: ${s.percentage}%`);
    const data = this.segmentData.map(s => s.count);
    
    this.pieChartData = {
      datasets: [{
        data: data,
        backgroundColor: ['#3498db', '#e74c3c', '#f39c12', '#2ecc71', '#9b59b6']
      }],
      labels: labels
    };
  }
}

