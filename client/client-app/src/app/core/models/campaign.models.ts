export interface Campaign {
  campaignId: number;
  campaignName: string;
  startDate: string;
  endDate: string;
  totalLeads?: number;
  openRate?: number;
  conversionRate?: number;
  status: string;
  buyerId?: number;
  agencyId?: number;
  brandId?: number;
}

export interface CreateCampaignRequest {
  campaignName: string;
  startDate: string;
  endDate: string;
  totalLeads?: number;
  status?: string;
  buyerId?: number;
  agencyId?: number;
  brandId?: number;
}

export interface UpdateCampaignRequest {
  campaignId: number;
  campaignName: string;
  startDate?: string;
  endDate?: string;
  totalLeads?: number;
  openRate?: number;
  conversionRate?: number;
  status?: string;
  buyerId?: number;
  agencyId?: number;
  brandId?: number;
}

