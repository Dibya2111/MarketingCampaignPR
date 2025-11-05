import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Lead, CreateLeadRequest, UpdateLeadRequest, LeadResponse } from '../models/lead.models';

@Injectable({
  providedIn: 'root'
})
export class LeadService {
  private apiUrl = `${environment.apiBaseUrl}/lead`;

  constructor(private http: HttpClient) {}

  getLeads(campaignId?: number, segment?: string, search?: string, page = 1, pageSize = 20): Observable<LeadResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    if (campaignId) params = params.set('campaignId', campaignId.toString());
    if (segment) params = params.set('segment', segment);
    if (search) params = params.set('search', search);

    return this.http.get<LeadResponse>(this.apiUrl, { params });
  }

  getLeadById(id: number): Observable<Lead> {
    return this.http.get<Lead>(`${this.apiUrl}/${id}`);
  }

  createLead(lead: CreateLeadRequest): Observable<any> {
    return this.http.post<any>(this.apiUrl, lead);
  }

  updateLead(lead: UpdateLeadRequest): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${lead.leadId}`, lead);
  }

  deleteLead(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  previewSegment(email: string, phone?: string): Observable<{ segment: string }> {
    return this.http.post<{ segment: string }>(`${this.apiUrl}/auto-segment-preview`, { email, phone });
  }
}

