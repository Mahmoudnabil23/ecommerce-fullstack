import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import {
  CartResponse,
  CartItem,
  AddCartItemRequest,
  UpdateCartItemRequest,
} from '../models/cart.model';

@Injectable({ providedIn: 'root' })
export class CartService {
  private http = inject(HttpClient);
  private platformId = inject(PLATFORM_ID);

  private readonly API = `${environment.apiBaseUrl}/cart`;
  private readonly GUEST_KEY = 'guest_session_id';

  private _cart = new BehaviorSubject<CartResponse | null>(null);
  cart$ = this._cart.asObservable();

  private _cartItemCount = new BehaviorSubject<number>(0);
  cartItemCount$ = this._cartItemCount.asObservable();

  /** Get or create a guest session ID */
  getGuestSessionId(): string {
    if (!isPlatformBrowser(this.platformId)) return '';
    let id = localStorage.getItem(this.GUEST_KEY);
    if (!id) {
      id = crypto.randomUUID();
      localStorage.setItem(this.GUEST_KEY, id);
    }
    return id;
  }

  clearGuestSession(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(this.GUEST_KEY);
    }
  }

  loadCart(): Observable<ApiResponse<CartResponse>> {
    return this.http.get<ApiResponse<CartResponse>>(this.API).pipe(
      tap((res) => {
        if (res.success && res.data) {
          this._cart.next(res.data);
          this._cartItemCount.next((res.data.items ?? []).reduce((sum, item) => sum + item.quantity, 0));
        }
      })
    );
  }

  addItem(request: AddCartItemRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.API}/items`, request).pipe(
      tap(() => this.loadCart().subscribe())
    );
  }

  updateItem(itemId: string, request: UpdateCartItemRequest): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.API}/items/${itemId}`, request).pipe(
      tap(() => this.loadCart().subscribe())
    );
  }

  removeItem(itemId: string): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.API}/items/${itemId}`).pipe(
      tap(() => this.loadCart().subscribe())
    );
  }
}
