import { PaymentMethod } from './order.model';

// ── Payment Request ──────────────────────────────────────────────────────
export interface InitiatePaymentRequest {
  orderId: string;
  paymentMethod: PaymentMethod;
  currency?: string; // default "EGP"
}

// ── Payment Response ─────────────────────────────────────────────────────
export interface PaymentInitiateResponse {
  clientSecret: string;
  paymentIntentId: string;
}
