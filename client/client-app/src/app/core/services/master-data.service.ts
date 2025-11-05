import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MasterItem } from '../models/master-data.models';

@Injectable({
  providedIn: 'root'
})
export class MasterDataService {
  private apiUrl = `${environment.apiBaseUrl}/masterdata`;

  constructor(private http: HttpClient) {}

  getBuyers(): Observable<MasterItem[]> {
    return this.http.get<MasterItem[]>(`${this.apiUrl}/buyers`);
  }

  getAgencies(): Observable<MasterItem[]> {
    return this.http.get<MasterItem[]>(`${this.apiUrl}/agencies`);
  }

  getBrands(): Observable<MasterItem[]> {
    return this.http.get<MasterItem[]>(`${this.apiUrl}/brands`);
  }

  getSegments(): Observable<MasterItem[]> {
    return this.http.get<MasterItem[]>(`${this.apiUrl}/segments`);
  }
}

