import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environment/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth';

const STORAGE_KEY = 'orgchart.auth';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private api = `${environment.apiUrl}/auth`;

  // Current session (null when logged out). Signal so the UI reacts (zoneless).
  private session = signal<AuthResponse | null>(this.readSession());

  readonly user = this.session.asReadonly();
  readonly isAuthenticated = computed(() => this.session() !== null);
  readonly isAdmin = computed(() => this.session()?.role === 'Admin');
  readonly username = computed(() => this.session()?.username ?? '');
  readonly role = computed(() => this.session()?.role ?? '');

  constructor(private http: HttpClient) {}

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.api}/login`, credentials).pipe(
      tap(res => this.setSession(res))
    );
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    // Admin-only endpoint; token is attached by the interceptor.
    return this.http.post<AuthResponse>(`${this.api}/register`, request);
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this.session.set(null);
  }

  getToken(): string | null {
    return this.session()?.token ?? null;
  }

  private setSession(res: AuthResponse): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(res));
    this.session.set(res);
  }

  private readSession(): AuthResponse | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;

    try {
      const session = JSON.parse(raw) as AuthResponse;

      // Drop it if the token has expired.
      if (session.expiresAt && new Date(session.expiresAt).getTime() <= Date.now()) {
        localStorage.removeItem(STORAGE_KEY);
        return null;
      }

      return session;
    } catch {
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
  }
}
