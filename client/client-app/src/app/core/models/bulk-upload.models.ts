export interface BulkLeadRow {
  email: string;
  name: string;
  phone?: string;
  campaignId?: number;
  segment?: string;
}

export interface BulkUploadRequest {
  rows: BulkLeadRow[];
  forceCampaignId?: number;
}

export interface BulkUploadDetail {
  detailId?: number;
  leadEmail?: string;
  validationStatus?: string;
  message?: string;
  createdDate?: string;
}

export interface BulkUploadLog {
  uploadId: number;
  uploadedBy: number;
  uploadedAt: string;
  totalRecords: number;
  validRecords: number;
  invalidRecords: number;
}

export interface BulkUploadResponse {
  uploadId: number;
  uploadedBy: number;
  uploadedAt: string;
  totalRecords: number;
  validRecords: number;
  invalidRecords: number;
}

