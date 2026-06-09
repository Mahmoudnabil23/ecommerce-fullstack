import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { InitiatePaymentRequest, PaymentInitiateResponse } from '../models/payment.model';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private http = inject(HttpClient);
  private readonly API = `${environment.apiBaseUrl}/payments`;

  initiatePayment(request: InitiatePaymentRequest): Observable<ApiResponse<PaymentInitiateResponse>> {
    return this.http.post<ApiResponse<PaymentInitiateResponse>>(`${this.API}/initiate`, request);
  }
}
