import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { finalize } from 'rxjs';
import { OrderService } from '../../../core/services/order.service';
import { ToastService } from '../../../core/services/toast.service';
import {
  OrderDetail,
  OrderStatus,
  PaymentMethod,
  OrderItem,
} from '../../../core/models/order.model';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="section">
      <div class="container small-container">
        <nav class="breadcrumb">
          <a routerLink="/profile" class="breadcrumb-item">My Account</a>
          <span class="breadcrumb-separator">/</span>
          <a routerLink="/profile/orders" class="breadcrumb-item">Orders</a>
          <span class="breadcrumb-separator">/</span>
          <span class="breadcrumb-item active">Details</span>
        </nav>

        @if (isLoading) {
          <div class="loading-state">
            <div class="spinner"></div>
            <p>Loading order details...</p>
          </div>
        } @else if (!order) {
          <div class="empty-state card">
            <h2 class="heading-4">Order not found</h2>
            <p class="text-muted">The order does not exist or you do not have access to it.</p>
            <a routerLink="/profile/orders" class="btn btn-primary">Back to Orders</a>
          </div>
        } @else {
          <header class="page-header">
            <div>
              <h1 class="heading-2">Order {{ order.orderNumber }}</h1>
              <p class="text-muted">Placed order details, status timeline, and shipping info.</p>
            </div>
            <span class="status-badge" [attr.data-status]="getStatusText(order.status).toLowerCase()">
              {{ getStatusText(order.status) }}
            </span>
          </header>

          <section class="card detail-card">
            <h2 class="heading-4">Items</h2>
            <div class="items-list">
              @for (item of order.items; track trackItem($index, item)) {
                <div class="item-row">
                  <div>
                    <p class="item-name">{{ item.name }}</p>
                    <p class="text-muted caption">Qty: {{ item.qty }}</p>
                  </div>
                  <div class="text-right">
                    <p class="item-price">EGP {{ item.unitPrice | number:'1.2-2' }}</p>
                    <p class="text-muted caption">Subtotal: EGP {{ item.qty * item.unitPrice | number:'1.2-2' }}</p>
                  </div>
                </div>
              }
            </div>
            <div class="total-row">
              <span>Total</span>
              <strong class="text-neon">EGP {{ order.total | number:'1.2-2' }}</strong>
            </div>
          </section>

          <section class="grid grid-2 detail-grid">
            <article class="card detail-card">
              <h2 class="heading-4">Shipping Address</h2>
              <p>{{ order.address.street }}</p>
              <p>{{ order.address.city }}</p>
              @if (order.address.postalCode) {
                <p class="text-muted">Postal Code: {{ order.address.postalCode }}</p>
              }
              @if (order.trackingNumber) {
                <p class="mt-3"><strong>Tracking:</strong> {{ order.trackingNumber }}</p>
              }
            </article>

            <article class="card detail-card">
              <h2 class="heading-4">Payment</h2>
              <p><strong>Method:</strong> {{ getPaymentMethodText(order.paymentMethod) }}</p>
              <p><strong>Status:</strong> {{ order.paymentStatus }}</p>
            </article>
          </section>

          <section class="card detail-card">
            <h2 class="heading-4">Status Timeline</h2>
            <div class="timeline">
              @for (event of order.statusHistory; track $index) {
                <div class="timeline-item">
                  <div class="timeline-dot"></div>
                  <div>
                    <p class="timeline-status">{{ getStatusText(event.status) }}</p>
                    <p class="text-muted caption">{{ event.timestamp | date:'medium' }}</p>
                  </div>
                </div>
              }
            </div>
          </section>
        }
      </div>
    </div>
  `,
  styles: [`
    .small-container { max-width: 980px; }

    .breadcrumb {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      margin-bottom: var(--space-5);
      font-size: 14px;
    }

    .breadcrumb-item { color: var(--color-shade-30); text-decoration: none; }
    .breadcrumb-item:hover { color: var(--color-neon-green); }
    .breadcrumb-item.active { color: var(--color-white); }
    .breadcrumb-separator { color: var(--color-shade-50); }

    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: var(--space-4);
      margin-bottom: var(--space-6);
    }

    .detail-card {
      padding: var(--space-5);
      margin-bottom: var(--space-5);
    }

    .items-list {
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
    }

    .item-row {
      display: flex;
      justify-content: space-between;
      gap: var(--space-4);
      padding-bottom: var(--space-3);
      border-bottom: 1px solid var(--color-shade-70);
    }

    .item-name { font-weight: 600; }
    .item-price { font-weight: 500; }

    .total-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-top: var(--space-4);
      font-size: 18px;
    }

    .detail-grid {
      gap: var(--space-5);
      margin-bottom: var(--space-5);
    }

    .timeline {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
    }

    .timeline-item {
      display: flex;
      align-items: flex-start;
      gap: var(--space-3);
    }

    .timeline-dot {
      width: 10px;
      height: 10px;
      border-radius: 50%;
      margin-top: 7px;
      background: var(--color-neon-green);
      box-shadow: 0 0 0 4px rgba(54, 244, 164, 0.15);
    }

    .timeline-status { font-weight: 500; }

    .status-badge {
      display: inline-flex;
      align-items: center;
      padding: 4px 12px;
      border-radius: var(--radius-pill);
      font-size: 12px;
      font-weight: 600;
      background: var(--color-dark-forest);
      color: var(--color-white);
    }

    .status-badge[data-status="delivered"],
    .status-badge[data-status="completed"] {
      background: rgba(54, 244, 164, 0.1);
      color: var(--color-neon-green);
    }

    .status-badge[data-status="cancelled"] {
      background: rgba(239, 68, 68, 0.1);
      color: #ef4444;
    }

    .status-badge[data-status="pending"] {
      background: rgba(245, 158, 11, 0.1);
      color: #f59e0b;
    }

    .loading-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 100px 0;
      gap: var(--space-4);
    }

    .empty-state {
      text-align: center;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-3);
      padding: 60px var(--space-5);
    }

    @media (max-width: 768px) {
      .page-header { flex-direction: column; align-items: flex-start; }
      .item-row { flex-direction: column; }
      .detail-grid { grid-template-columns: 1fr; }
    }
  `],
})
export class OrderDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private orderService = inject(OrderService);
  private toast = inject(ToastService);

  order: OrderDetail | null = null;
  isLoading = true;

  ngOnInit(): void {
    const orderId = this.route.snapshot.paramMap.get('id');
    if (!orderId) {
      this.isLoading = false;
      this.toast.error('Invalid order id');
      return;
    }

    this.orderService
      .getOrderDetails(orderId)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.order = response.data;
            return;
          }

          this.toast.error(response.message || 'Order not found');
        },
        error: () => {
          this.toast.error('Failed to load order details');
        },
      });
  }

  getStatusText(status: OrderStatus): string {
    return OrderStatus[status] || 'Unknown';
  }

  getPaymentMethodText(method: PaymentMethod): string {
    return PaymentMethod[method] || 'Unknown';
  }

  trackItem(index: number, item: OrderItem): string {
    return `${item.productId}-${index}`;
  }
}
