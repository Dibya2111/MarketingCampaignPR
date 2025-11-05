export interface CreateEngagementMetric {
  leadId: number;
  campaignId?: number;
  emailsSent: number;
  emailsOpened: number;
  clicks: number;
  conversions: number;
}

export interface UpdateEngagementMetric {
  metricId: number;
  emailsSent?: number;
  emailsOpened?: number;
  clicks?: number;
  conversions?: number;
  openRate?: number;
  clickRate?: number;
  conversionRate?: number;
}

export interface EngagementMetric {
  metricId: number;
  leadId?: number;
  campaignId?: number;
  openRate?: number;
  clickRate?: number;
  conversionRate?: number;
  createdDate?: string;
  lastModifiedDate?: string;
}

export interface CampaignMetrics {
  campaignId: number;
  totalEmailsSent: number;
  totalEmailsOpened: number;
  totalClicks: number;
  totalConversions: number;
  openRate: number;
  clickRate: number;
  conversionRate: number;
}

