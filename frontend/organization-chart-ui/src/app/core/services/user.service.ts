import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { User } from '../models/user';
import { environment } from '../../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private api = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<User[]> {
    return this.http.get<User[]>(this.api);
  }

  getById(id: number): Observable<User> {
    return this.http.get<User>(`${this.api}/${id}`);
  }

  create(user: User): Observable<User> {
    return this.http.post<User>(`${this.api}`, user);
  }

  update(id: number, user: User): Observable<void> {
    return this.http.put<void>(`${this.api}/${id}`, user);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/${id}`);
  }
}
