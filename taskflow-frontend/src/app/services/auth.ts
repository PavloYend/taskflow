import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;

  private tokenKey = 'token';

  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient) {}

  login(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, data, { responseType: 'text' });
  }

  private parseTokenResponse(response: string): string {
    const trimmed = response.trim();

    try {
      const parsed = JSON.parse(trimmed);
      if (parsed && typeof parsed === 'object') {
        const tokenField = parsed.text ?? parsed.token ?? parsed.accessToken;
        if (typeof tokenField === 'string' && tokenField.trim().length > 0) {
          return tokenField.trim();
        }
      }
    } catch {
      // response is not JSON, use raw string
    }

    return trimmed;
  }

  saveToken(token: string): void {
    const cleanToken = this.parseTokenResponse(token);
    localStorage.setItem(this.tokenKey, cleanToken);
    this.loggedIn.next(true);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.loggedIn.next(false);
  }

  isLoggedIn() {
    return this.loggedIn.asObservable();
  }

  isAuthenticated(): boolean {
    return this.loggedIn.value;
  }

  register(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data, { responseType: 'text' });
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }
}
