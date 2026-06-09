import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { ProductDetail } from '../models/product.model';
import {
  AdminDashboard,
  AdminUserListItem,
  AdminUserFilter,
  AdminProductFilter,
  UpdateUserStatusRequest,
  AdminPaginatedUsers,
  AdminPaginatedProducts,
} from '../models/user.model';
import { CategoryItem } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}`;

  // ── Dashboard ────────────────────────────────────────────────────────
  getDashboard(): Observable<ApiResponse<AdminDashboard>> {
    return this.http.get<ApiResponse<AdminDashboard>>(`${this.API}/admin/dashboard`);
  }

  // ── Product Management ───────────────────────────────────────────────
  getProducts(filter?: AdminProductFilter): Observable<ApiResponse<AdminPaginatedProducts>> {
    let params = new HttpParams();
    if (filter) {
      if (filter.search) params = params.set('search', filter.search);
      if (filter.page) params = params.set('page', filter.page.toString());
      if (filter.limit) params = params.set('limit', filter.limit.toString());
    }
    return this.http.get<ApiResponse<AdminPaginatedProducts>>(`${this.API}/admin/products`, { params });
  }

  getProductById(id: string): Observable<ApiResponse<ProductDetail>> {
    return this.http.get<ApiResponse<ProductDetail>>(`${this.API}/admin/products/${id}`);
  }

  deleteProduct(id: string): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.API}/admin/products/${id}`);
  }

  // ── User Management ──────────────────────────────────────────────────
  getUsers(filter?: AdminUserFilter): Observable<ApiResponse<AdminPaginatedUsers>> {
    let params = new HttpParams();
    if (filter) {
      if (filter.search) params = params.set('search', filter.search);
      if (filter.role) params = params.set('role', filter.role);
      if (filter.status != null) params = params.set('status', filter.status.toString());
      if (filter.page) params = params.set('page', filter.page.toString());
      if (filter.limit) params = params.set('limit', filter.limit.toString());
    }
    return this.http.get<ApiResponse<AdminPaginatedUsers>>(`${this.API}/admin/users`, { params });
  }

  getUserById(id: string): Observable<ApiResponse<AdminUserListItem>> {
    return this.http.get<ApiResponse<AdminUserListItem>>(`${this.API}/admin/users/${id}`);
  }

  updateUserStatus(id: string, request: UpdateUserStatusRequest): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.API}/admin/users/${id}/status`, request);
  }

  deleteUser(id: string): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.API}/admin/users/${id}`);
  }

  // ── Category Management ──────────────────────────────────────────────
  getCategories(): Observable<ApiResponse<CategoryItem[]>> {
    return this.http.get<ApiResponse<CategoryItem[]>>(`${this.API}/categories`);
  }
}
