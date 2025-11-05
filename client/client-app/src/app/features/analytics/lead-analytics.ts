import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EngagementMetricService } from '../../core/services/engagement-metric.service';
import { EngagementMetric } from '../../core/models/engagement-metric.models';

@Component({
  selector: 'app-lead-analytics',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './lead-analytics.html',
  styleUrls: ['./lead-analytics.css']
})
export class LeadAnalyticsComponent implements OnInit {
  @Input() leadId = 0;
  @Input() showModal = false;
  
  metrics: EngagementMetric[] = [];
  loading = false;
  error = '';

  constructor(private engagementService: EngagementMetricService) {}

  ngOnInit(): void {
    if (this.leadId && this.showModal) {
      this.loadMetrics();
    }
  }

  ngOnChanges(): void {
    if (this.leadId && this.showModal) {
      this.loadMetrics();
    }
  }

  loadMetrics(): void {
    this.loading = true;
    this.engagementService.getMetricsByLead(this.leadId).subscribe({
      next: (metrics) => {
        this.metrics = metrics;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load lead metrics';
        this.loading = false;
        console.error('Error loading lead metrics:', err);
      }
    });
  }

  closeModal(): void {
    this.showModal = false;
    this.showModalChange.emit(false);
  }

  @Output() showModalChange = new EventEmitter<boolean>();
}

