import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { timeout } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.model';
import {
  OrderListItem,
  OrderDetail
} from '../models/order.model';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/orders`;
  private readonly requestTimeoutMs = 10000;

  /**
   * Places a new order
   */
  placeOrder(orderData: any): Observable<ApiResponse<{ orderId: string, orderNumber: string }>> {
    return this.http.post<ApiResponse<{ orderId: string, orderNumber: string }>>(this.apiUrl, orderData);
  }

  /**
   * Fetches order history for current user
   */
  getUserOrders(page: number = 1, limit: number = 10): Observable<ApiResponse<OrderListItem[]>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('limit', limit.toString());

    return this.http
      .get<ApiResponse<OrderListItem[]>>(`${this.apiUrl}/me`, { params })
      .pipe(timeout(this.requestTimeoutMs));
  }

  /**
   * Fetches details for a specific order
   */
  getOrderDetails(orderId: string): Observable<ApiResponse<OrderDetail>> {
    return this.http
      .get<ApiResponse<OrderDetail>>(`${this.apiUrl}/${orderId}`)
      .pipe(timeout(this.requestTimeoutMs));
  }

  /**
   * Cancels a pending order
   */
  cancelOrder(orderId: string, reason?: string): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/${orderId}/cancel`, { reason });
  }
}
