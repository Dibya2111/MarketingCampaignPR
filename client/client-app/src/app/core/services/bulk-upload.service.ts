import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BulkUploadRequest, BulkUploadResponse, BulkUploadLog, BulkUploadDetail } from '../models/bulk-upload.models';

@Injectable({
  providedIn: 'root'
})
export class BulkUploadService {
  private apiUrl = `${environment.apiBaseUrl}/BulkUpload`;

  constructor(private http: HttpClient) {}

  uploadLeads(request: BulkUploadRequest): Observable<BulkUploadResponse> {
    return this.http.post<BulkUploadResponse>(`${this.apiUrl}/upload`, request);
  }

  getUploadLogs(): Observable<BulkUploadLog[]> {
    return this.http.get<BulkUploadLog[]>(`${this.apiUrl}/logs`);
  }

  getUploadDetails(uploadId: number): Observable<BulkUploadDetail[]> {
    return this.http.get<BulkUploadDetail[]>(`${this.apiUrl}/details/${uploadId}`);
  }
}

