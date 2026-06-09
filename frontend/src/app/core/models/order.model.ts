// ── Enums ────────────────────────────────────────────────────────────────
export enum OrderStatus {
  Pending = 0,
  Confirmed = 1,
  Processing = 2,
  Shipped = 3,
  Delivered = 4,
  Completed = 5,
  Cancelled = 6,
}

export enum PaymentMethod {
  Stripe = 0,
  PayPal = 1,
  CashOnDelivery = 2,
  Wallet = 3,
}

export enum PaymentStatus {
  Pending = 0,
  Completed = 1,
  Failed = 2,
}

// ── Order List Item ──────────────────────────────────────────────────────
export interface OrderListItem {
  orderId: string;
  orderNumber: string;
  status: OrderStatus;
  total: number;
  createdAt: string;
}

// ── Order Detail ─────────────────────────────────────────────────────────
export interface OrderAddress {
  street: string;
  city: string;
  postalCode?: string;
}

export interface OrderItem {
  productId: string;
  name: string;
  qty: number;
  unitPrice: number;
}

export interface OrderStatusHistory {
  status: OrderStatus;
  timestamp: string;
}

export interface OrderDetail {
  orderId: string;
  orderNumber: string;
  status: OrderStatus;
  items: OrderItem[];
  address: OrderAddress;
  paymentMethod: PaymentMethod;
  paymentStatus: string;
  trackingNumber?: string;
  statusHistory: OrderStatusHistory[];
  total: number;
}
