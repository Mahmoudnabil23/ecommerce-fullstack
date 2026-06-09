import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { finalize } from 'rxjs';
import { OrderService } from '../../../core/services/order.service';
import { OrderListItem, OrderStatus } from '../../../core/models/order.model';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="section">
      <div class="container small-container">
        <nav class="breadcrumb">
          <a routerLink="/profile" class="breadcrumb-item">My Account</a>
          <span class="breadcrumb-separator">/</span>
          <span class="breadcrumb-item active">Orders</span>
        </nav>

        <header class="page-header">
          <h1 class="heading-2">Order History</h1>
          <p class="text-muted">Check the status of recent orders, manage returns, and discover similar products.</p>
        </header>

        @if (isLoading) {
          <div class="loading-state">
            <div class="spinner"></div>
            <p>Loading your orders...</p>
          </div>
        } @else if (orders.length === 0) {
          <div class="empty-state card">
            <div class="empty-icon">
              <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M6 2L3 6v14a2 2 0 002 2h14a2 2 0 002-2V6l-3-4z"/><line x1="3" y1="6" x2="21" y2="6"/></svg>
            </div>
            <h2 class="heading-4">No orders yet</h2>
            <p class="text-muted">You haven't placed any orders yet. Start shopping to see your orders here!</p>
            <a routerLink="/products" class="btn btn-primary btn-lg mt-4">Start Shopping</a>
          </div>
        } @else {
          <div class="orders-list">
            @for (order of orders; track order.orderId) {
              <div class="order-card card">
                <div class="order-header">
                  <div class="order-meta">
                    <div class="meta-item">
                      <span class="meta-label">Order Number</span>
                      <span class="meta-value">{{ order.orderNumber }}</span>
                    </div>
                    <div class="meta-item">
                      <span class="meta-label">Placed On</span>
                      <span class="meta-value">{{ order.createdAt | date:'mediumDate' }}</span>
                    </div>
                    <div class="meta-item">
                      <span class="meta-label">Total Amount</span>
                      <span class="meta-value text-neon">EGP {{ order.total | number:'1.2-2' }}</span>
                    </div>
                  </div>
                  <div class="order-actions">
                    <span class="status-badge" [attr.data-status]="getStatusText(order.status).toLowerCase()">
                      {{ getStatusText(order.status) }}
                    </span>
                    <a class="btn btn-ghost btn-sm" [routerLink]="['/profile/orders', order.orderId]">View Details</a>
                  </div>
                </div>
              </div>
            }
          </div>
          
          @if (totalCount > limit) {
             <div class="pagination">
                <!-- Pagination logic if needed -->
             </div>
          }
        }
      </div>
    </div>
  `,
  styles: [`
    .small-container { max-width: 900px; }
    
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

    .page-header { margin-bottom: var(--space-8); }
    .page-header p { margin-top: var(--space-2); }

    .loading-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 100px 0;
      gap: var(--space-4);
    }

    .empty-state {
      padding: 80px var(--space-5);
      text-align: center;
      display: flex;
      flex-direction: column;
      align-items: center;
    }

    .empty-icon {
      color: var(--color-shade-50);
      margin-bottom: var(--space-4);
    }

    .orders-list {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
    }

    .order-card {
      padding: var(--space-5);
      transition: transform var(--transition-fast);
    }

    .order-card:hover {
      border-color: var(--color-shade-50);
    }

    .order-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      gap: var(--space-4);
    }

    .order-meta {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(130px, 1fr));
      gap: var(--space-6);
      flex: 1;
    }

    .meta-item {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .meta-label {
      font-size: 12px;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: var(--color-shade-40);
    }

    .meta-value {
      font-weight: 500;
      color: var(--color-white);
    }

    .order-actions {
      display: flex;
      flex-direction: column;
      align-items: flex-end;
      gap: var(--space-3);
    }

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

    @media (max-width: 640px) {
      .order-header { flex-direction: column; }
      .order-actions { align-items: flex-start; width: 100%; border-top: 1px solid var(--color-shade-70); padding-top: var(--space-4); }
      .order-meta { grid-template-columns: 1fr 1fr; width: 100%; }
    }
  `]
})
export class OrderHistoryComponent implements OnInit {
  private orderService = inject(OrderService);
  private toast = inject(ToastService);

  orders: OrderListItem[] = [];
  isLoading = true;
  page = 1;
  limit = 10;
  totalCount = 0;

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.isLoading = true;
    this.orderService.getUserOrders(this.page, this.limit)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.orders = response.data;
            // total count handled here if API returned it in a wrapper or meta
          }
        },
        error: (error) => {
          if (error?.name === 'TimeoutError') {
            this.toast.error('Orders request timed out. Please check backend server and try again.');
          } else {
            this.toast.error('Failed to load order history');
          }
          console.error('Order history error:', error);
        }
      });
  }

  getStatusText(status: OrderStatus): string {
    return OrderStatus[status] || 'Unknown';
  }
}
