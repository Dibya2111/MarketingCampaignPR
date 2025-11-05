import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CampaignPerformanceSnapshot } from '../models/campaign-performance.models';

@Injectable({
  providedIn: 'root'
})
export class CampaignPerformanceService {
  private apiUrl = `${environment.apiBaseUrl}/CampaignPerformanceSnapshot`;

  constructor(private http: HttpClient) {}

  createSnapshot(campaignId: number): Observable<CampaignPerformanceSnapshot> {
    return this.http.post<CampaignPerformanceSnapshot>(`${this.apiUrl}/create/${campaignId}`, {});
  }

  getSnapshotsByCampaign(campaignId: number): Observable<CampaignPerformanceSnapshot[]> {
    return this.http.get<CampaignPerformanceSnapshot[]>(`${this.apiUrl}/campaign/${campaignId}`);
  }

  getAllSnapshots(): Observable<CampaignPerformanceSnapshot[]> {
    return this.http.get<CampaignPerformanceSnapshot[]>(`${this.apiUrl}/all`);
  }
}

