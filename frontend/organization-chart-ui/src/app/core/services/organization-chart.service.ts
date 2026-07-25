import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { OrganizationChartNode } from '../models/organization-chart';
import { environment } from '../../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class OrganizationChartService {

  private api = `${environment.apiUrl}/organizationchart`;

  constructor(private http: HttpClient) {}

  getChart(): Observable<OrganizationChartNode[]> {
    return this.http.get<OrganizationChartNode[]>(this.api);
  }
}
