import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { EngagementMetricService } from '../../core/services/engagement-metric.service';
import { CampaignService } from '../../core/services/campaign.service';
import { CampaignPerformanceService } from '../../core/services/campaign-performance.service';
import { Campaign } from '../../core/models/campaign.models';
import { CampaignPerformanceSnapshot } from '../../core/models/campaign-performance.models';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartType } from 'chart.js';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-campaign-dashboard',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './campaign-dashboard.html',
  styleUrls: ['./campaign-dashboard.css']
})
export class CampaignDashboardComponent implements OnInit {
  campaign?: Campaign;
  metrics: any = {};
  snapshots: CampaignPerformanceSnapshot[] = [];
  loading = false;
  error = '';
  campaignId = 0;

  public lineChartData: ChartConfiguration['data'] = {
    datasets: [
      { data: [], label: 'Open Rate', borderColor: '#3498db', backgroundColor: 'rgba(52, 152, 219, 0.1)' },
      { data: [], label: 'Conversion Rate', borderColor: '#e74c3c', backgroundColor: 'rgba(231, 76, 60, 0.1)' }
    ],
    labels: []
  };
  public lineChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    scales: {
      y: { beginAtZero: true, max: 100 }
    }
  };
  public lineChartType: ChartType = 'line';

  constructor(
    private route: ActivatedRoute,
    private engagementService: EngagementMetricService,
    private campaignService: CampaignService,
    private performanceService: CampaignPerformanceService
  ) {}

  ngOnInit(): void {
    this.campaignId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.campaignId) {
      this.loadCampaignDetails();
      this.loadMetrics();
      this.loadSnapshots();
    }
  }

  loadCampaignDetails(): void {
    this.campaignService.getCampaignById(this.campaignId).subscribe({
      next: (campaign) => this.campaign = campaign,
      error: (err) => console.error('Error loading campaign:', err)
    });
  }

  loadMetrics(): void {
    this.loading = true;
    this.engagementService.getMetricsByCampaign(this.campaignId).subscribe({
      next: (data) => {
        this.metrics = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load metrics';
        this.loading = false;
        console.error('Error loading metrics:', err);
      }
    });
  }

  loadSnapshots(): void {
    this.performanceService.getSnapshotsByCampaign(this.campaignId).subscribe({
      next: (snapshots) => {
        this.snapshots = snapshots;
        this.updateChart();
      },
      error: (err) => console.error('Error loading snapshots:', err)
    });
  }

  updateChart(): void {
    const labels = this.snapshots.map(s => new Date(s.dateCaptured!).toLocaleDateString());
    const openRates = this.snapshots.map(s => s.openRate || 0);
    const conversionRates = this.snapshots.map(s => s.conversionRate || 0);

    this.lineChartData = {
      datasets: [
        { data: openRates, label: 'Open Rate (%)', borderColor: '#3498db', backgroundColor: 'rgba(52, 152, 219, 0.1)' },
        { data: conversionRates, label: 'Conversion Rate (%)', borderColor: '#e74c3c', backgroundColor: 'rgba(231, 76, 60, 0.1)' }
      ],
      labels: labels
    };
  }

  recalculateSnapshot(): void {
    this.loading = true;
    this.performanceService.createSnapshot(this.campaignId).subscribe({
      next: () => {
        this.loadSnapshots();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error recalculating snapshot:', err);
        this.loading = false;
      }
    });
  }
}

