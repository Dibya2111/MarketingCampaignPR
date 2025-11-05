export interface CampaignPerformanceSnapshot {
  snapshotId: number;
  campaignId: number;
  totalLeads?: number;
  openRate?: number;
  conversionRate?: number;
  dateCaptured?: string;
}

export interface CreateCampaignPerformanceSnapshot {
  campaignId: number;
}

