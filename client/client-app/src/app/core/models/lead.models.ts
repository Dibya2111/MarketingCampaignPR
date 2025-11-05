export interface Lead {
  leadId: number;
  name: string;
  email: string;
  phone?: string;
  campaignId?: number;
  segment?: string;
  createdByUserId?: number;
  createdDate?: string;
  lastModifiedUserId?: number;
  lastModifiedDate?: string;
  isActive?: boolean;
  isDeleted?: boolean;
}

export interface CreateLeadRequest {
  name: string;
  email: string;
  phone?: string;
  campaignId?: number;
  segment?: string;
}

export interface UpdateLeadRequest {
  leadId: number;
  name?: string;
  email?: string;
  phone?: string;
  campaignId?: number;
  segment?: string;
  isActive?: boolean;
}

export interface LeadResponse {
  items: Lead[];
  total: number;
}

