import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateEngagementMetric, UpdateEngagementMetric, EngagementMetric } from '../models/engagement-metric.models';

@Injectable({
  providedIn: 'root'
})
export class EngagementMetricService {
  private apiUrl = `${environment.apiBaseUrl}/EngagementMetric`;

  constructor(private http: HttpClient) {}

  createMetric(metric: CreateEngagementMetric): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/create`, metric);
  }

  updateMetric(metric: UpdateEngagementMetric): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${metric.metricId}`, metric);
  }

  deleteMetric(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  getMetricsByCampaign(campaignId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/campaign/${campaignId}`);
  }

  getMetricsByLead(leadId: number): Observable<EngagementMetric[]> {
    return this.http.get<EngagementMetric[]>(`${this.apiUrl}/lead/${leadId}`);
  }
}

