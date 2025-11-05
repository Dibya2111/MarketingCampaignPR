import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { GenerateOtpRequest, VerifyOtpRequest, OtpResponse } from '../models/otp.models';

@Injectable({
  providedIn: 'root'
})
export class OtpService {
  private apiUrl = `${environment.apiBaseUrl}/Otp`;

  constructor(private http: HttpClient) {}

  generateOtp(request: GenerateOtpRequest): Observable<OtpResponse> {
    return this.http.post<OtpResponse>(`${this.apiUrl}/generate`, request);
  }

  verifyOtp(request: VerifyOtpRequest): Observable<OtpResponse> {
    return this.http.post<OtpResponse>(`${this.apiUrl}/verify`, request);
  }
}

