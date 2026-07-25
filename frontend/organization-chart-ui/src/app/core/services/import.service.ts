import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environment/environment';
@Injectable({
  providedIn: 'root'
})
export class ImportService {

  private api = `${environment.apiUrl}/import`;

  constructor(private http: HttpClient) {}

  importDepartments(file: File): Observable<void> {

    const formData = new FormData();

    formData.append('file', file);

    return this.http.post<void>(
      `${this.api}/departments`,
      formData
    );
  }

  importUsers(file: File): Observable<void> {

    const formData = new FormData();

    formData.append('file', file);

    return this.http.post<void>(
      `${this.api}/users`,
      formData
    );
  }

}