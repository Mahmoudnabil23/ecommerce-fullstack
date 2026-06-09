import { Injectable, signal, computed, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError, BehaviorSubject } from 'rxjs';
import { environment } from '../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  RegisterResponse,
  RefreshTokenRequest,
  RefreshTokenResponse,
  UserSummary,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  ChangePasswordRequest,
  GoogleAuthRequest,
} from '../models/auth.model';
import { UserRole } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private platformId = inject(PLATFORM_ID);

  private readonly API = `${environment.apiBaseUrl}/auth`;
  private readonly TOKEN_KEY = 'access_token';
  private readonly REFRESH_KEY = 'refresh_token';
  private readonly USER_KEY = 'current_user';

  private _currentUser = new BehaviorSubject<UserSummary | null>(this.loadUser());
  currentUser$ = this._currentUser.asObservable();

  get currentUser(): UserSummary | null {
    return this._currentUser.value;
  }

  get isLoggedIn(): boolean {
    return !!this.getToken();
  }

  get isAdmin(): boolean {
    return this.hasRole(UserRole.Admin);
  }

  get isSeller(): boolean {
    return this.hasRole(UserRole.Seller);
  }

  get isCustomer(): boolean {
    return this.hasRole(UserRole.Customer);
  }

  hasRole(role: UserRole | string): boolean {
    const targetRole = role.toString().toLowerCase();
    return this.currentUser?.roles?.some((r) => r.toLowerCase() === targetRole) ?? false;
  }

  getDefaultRoute(): string {
    if (!this.isLoggedIn) return '/welcome';
    if (this.isAdmin) return '/admin/dashboard';
    if (this.isSeller) return '/seller/dashboard';
    return '/customer/home';
  }

  // ── Auth Actions ─────────────────────────────────────────────────────
  login(request: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.API}/login`, request).pipe(
      tap((res) => {
        if (res.success && res.data) {
          this.storeTokens(res.data);
          this.storeUser(res.data.user);
        }
      })
    );
  }

  register(request: RegisterRequest): Observable<ApiResponse<RegisterResponse>> {
    return this.http.post<ApiResponse<RegisterResponse>>(`${this.API}/register`, request);
  }

  refreshToken(): Observable<ApiResponse<RefreshTokenResponse>> {
    const refreshToken = this.getRefreshToken();
    return this.http
      .post<ApiResponse<RefreshTokenResponse>>(`${this.API}/refresh-token`, {
        refreshToken,
      } as RefreshTokenRequest)
      .pipe(
        tap((res) => {
          if (res.success && res.data) {
            this.setToken(res.data.accessToken);
            this.setRefreshToken(res.data.refreshToken);
          }
        }),
        catchError((err) => {
          this.logout(false);
          return throwError(() => err);
        })
      );
  }

  forgotPassword(request: ForgotPasswordRequest): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(`${this.API}/forgot-password`, request);
  }

  resetPassword(request: ResetPasswordRequest): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(`${this.API}/reset-password`, request);
  }

  changePassword(request: ChangePasswordRequest): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(`${this.API}/change-password`, request);
  }

  googleAuth(request: GoogleAuthRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.API}/google`, request).pipe(
      tap((res) => {
        if (res.success && res.data) {
          this.storeTokens(res.data);
          this.storeUser(res.data.user);
        }
      })
    );
  }

  verifyEmail(userId: string, token: string): Observable<ApiResponse<null>> {
    const params = new HttpParams().set('userId', userId).set('token', token);
    return this.http.get<ApiResponse<null>>(`${this.API}/verify-email`, { params });
  }

  logout(callApi: boolean = true): void {
    const refreshToken = this.getRefreshToken();
    if (callApi && refreshToken) {
      this.http.post<ApiResponse<null>>(`${this.API}/logout`, { refreshToken }).subscribe({ error: () => {} });
    }
    this.clearLocalAuth();
    this.router.navigate(['/login']);
  }

  clearLocalAuth(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(this.TOKEN_KEY);
      localStorage.removeItem(this.REFRESH_KEY);
      localStorage.removeItem(this.USER_KEY);
    }
    this._currentUser.next(null);
  }

  // ── Token Helpers ────────────────────────────────────────────────────
  getToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return localStorage.getItem(this.REFRESH_KEY);
  }

  private setToken(token: string): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(this.TOKEN_KEY, token);
    }
  }

  private setRefreshToken(token: string): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(this.REFRESH_KEY, token);
    }
  }

  private storeTokens(auth: AuthResponse): void {
    this.setToken(auth.accessToken);
    this.setRefreshToken(auth.refreshToken);
  }

  private storeUser(user: UserSummary): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    }
    this._currentUser.next(user);
  }

  private loadUser(): UserSummary | null {
    if (typeof window === 'undefined') return null;
    const raw = localStorage.getItem(this.USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }
}
