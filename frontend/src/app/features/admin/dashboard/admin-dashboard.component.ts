import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';
import { AdminService } from '../../../core/services/admin.service';
import { AdminDashboard, SalesByDay } from '../../../core/models/user.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, LoadingSpinnerComponent],
  template: `
    @if (isLoading) {
      <app-loading-spinner />
    } @else if (dashboard) {
      <div class="dashboard">
        <h1 class="heading-3" style="margin-bottom: var(--space-7);">Dashboard</h1>

        <!-- Stats Cards -->
        <div class="stats-cards">
          <div class="stat-card card">
            <div class="stat-card-icon" style="background: rgba(54, 244, 164, 0.1); color: #36F4A4;">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="12" y1="1" x2="12" y2="23"/><path d="M17 5H9.5a3.5 3.5 0 000 7h5a3.5 3.5 0 010 7H6"/></svg>
            </div>
            <div>
              <p class="caption text-muted">Total Revenue</p>
              <p class="heading-4">EGP {{ dashboard.totalRevenue | number:'1.2-2' }}</p>
            </div>
          </div>

          <div class="stat-card card">
            <div class="stat-card-icon" style="background: rgba(96, 165, 250, 0.1); color: #60A5FA;">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M6 2L3 6v14a2 2 0 002 2h14a2 2 0 002-2V6l-3-4z"/><line x1="3" y1="6" x2="21" y2="6"/></svg>
            </div>
            <div>
              <p class="caption text-muted">Total Orders</p>
              <p class="heading-4">{{ dashboard.totalOrders | number }}</p>
            </div>
          </div>

          <div class="stat-card card">
            <div class="stat-card-icon" style="background: rgba(251, 191, 36, 0.1); color: #FBBF24;">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2"/><circle cx="9" cy="7" r="4"/></svg>
            </div>
            <div>
              <p class="caption text-muted">Total Users</p>
              <p class="heading-4">{{ dashboard.totalUsers | number }}</p>
            </div>
          </div>

          <div class="stat-card card">
            <div class="stat-card-icon" style="background: rgba(192, 132, 252, 0.1); color: #C084FC;">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="2" y="7" width="20" height="14" rx="2"/><path d="M16 21V5a2 2 0 00-2-2h-4a2 2 0 00-2 2v16"/></svg>
            </div>
            <div>
              <p class="caption text-muted">Total Products</p>
              <p class="heading-4">{{ dashboard.totalProducts | number }}</p>
            </div>
          </div>
        </div>

        <!-- Recent Orders -->
        <div class="dashboard-section">
          <h2 class="heading-5" style="margin-bottom: var(--space-5);">Recent Orders</h2>
          <div class="table-card card">
            <table class="admin-table">
              <thead>
                <tr>
                  <th>Order #</th>
                  <th>Date</th>
                  <th>Status</th>
                  <th>Total</th>
                </tr>
              </thead>
              <tbody>
                @for (order of dashboard.recentOrders; track order.orderId) {
                  <tr>
                    <td class="body-small">{{ order.orderNumber }}</td>
                    <td class="caption text-muted">{{ order.createdAt | date:'mediumDate' }}</td>
                    <td><span class="status-badge" [class]="'status-' + order.status">{{ getStatusLabel(order.status) }}</span></td>
                    <td class="body-small">EGP {{ order.total | number:'1.2-2' }}</td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        </div>

        <!-- Top Products -->
        <div class="dashboard-section">
          <h2 class="heading-5" style="margin-bottom: var(--space-5);">Top Products</h2>
          <div class="table-card card">
            <table class="admin-table">
              <thead>
                <tr>
                  <th>Name</th>
                  <th>Price</th>
                  <th>Stock</th>
                  <th>Rating</th>
                </tr>
              </thead>
              <tbody>
                @for (product of dashboard.topProducts; track product.id) {
                  <tr>
                    <td class="body-small">{{ product.name }}</td>
                    <td class="body-small">EGP {{ product.price | number:'1.2-2' }}</td>
                    <td class="caption" [class.text-neon]="product.stock > 0" [style.color]="product.stock <= 0 ? '#ef4444' : ''">{{ product.stock }}</td>
                    <td class="caption text-muted">{{ product.averageRating | number:'1.1-1' }} ⭐ ({{ product.reviewCount }})</td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    .stats-cards {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: var(--space-5);
      margin-bottom: var(--space-9);
    }

    .stat-card {
      padding: var(--space-5);
      display: flex;
      align-items: center;
      gap: var(--space-4);
    }

    .stat-card-icon {
      width: 48px;
      height: 48px;
      border-radius: var(--radius-md);
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }

    .dashboard-section {
      margin-bottom: var(--space-9);
    }

    .table-card {
      overflow-x: auto;
    }

    .admin-table {
      width: 100%;
      border-collapse: collapse;
    }

    .admin-table th {
      text-align: left;
      padding: var(--space-4) var(--space-5);
      font-size: 12px;
      font-weight: 500;
      color: var(--color-muted);
      text-transform: uppercase;
      letter-spacing: 0.72px;
      border-bottom: 1px solid var(--color-shade-70);
    }

    .admin-table td {
      padding: var(--space-4) var(--space-5);
      border-bottom: 1px solid var(--color-dark-card-border);
    }

    .admin-table tr:hover td {
      background: rgba(255,255,255,0.02);
    }

    .status-badge {
      font-size: 12px;
      font-weight: 500;
      padding: 4px 12px;
      border-radius: var(--radius-pill);
    }

    .status-0 { background: rgba(251, 191, 36, 0.1); color: #FBBF24; }
    .status-1 { background: rgba(96, 165, 250, 0.1); color: #60A5FA; }
    .status-2 { background: rgba(192, 132, 252, 0.1); color: #C084FC; }
    .status-3 { background: rgba(54, 244, 164, 0.1); color: #36F4A4; }
    .status-4 { background: rgba(54, 244, 164, 0.15); color: #36F4A4; }
    .status-5 { background: rgba(54, 244, 164, 0.2); color: #36F4A4; }
    .status-6 { background: rgba(239, 68, 68, 0.1); color: #ef4444; }

    @media (max-width: 1024px) {
      .stats-cards { grid-template-columns: repeat(2, 1fr); }
    }

    @media (max-width: 640px) {
      .stats-cards { grid-template-columns: 1fr; }
    }
  `]
})
export class AdminDashboardComponent implements OnInit {
  private adminService = inject(AdminService);

  dashboard: AdminDashboard | null = null;
  isLoading = true;

  private statusLabels: Record<number, string> = {
    0: 'Pending', 1: 'Confirmed', 2: 'Processing', 3: 'Shipped', 4: 'Delivered', 5: 'Completed', 6: 'Cancelled'
  };

  ngOnInit() {
    this.adminService.getDashboard()
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res) => {
          this.dashboard = res.data ?? null;
        },
        error: () => { console.error('Failed to load dashboard'); },
      });
  }

  getStatusLabel(status: number): string {
    return this.statusLabels[status] ?? 'Unknown';
  }
}
