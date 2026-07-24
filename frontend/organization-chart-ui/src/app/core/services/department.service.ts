import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Department } from '../models/department';
import { environment } from '../../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {

  private api = `${environment.apiUrl}/departments`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Department[]> {
    return this.http.get<Department[]>(this.api);
  }

  getById(id: number): Observable<Department> {
    return this.http.get<Department>(`${this.api}/${id}`);
  }

  create(department: Department): Observable<Department> {
    return this.http.post<Department>(`${this.api}`, department);
  }

  update(id: number, department: Department): Observable<void> {
    return this.http.put<void>(`${this.api}/${id}`, department);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }
}