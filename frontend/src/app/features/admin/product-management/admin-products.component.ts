import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { AdminService } from '../../../core/services/admin.service';
import { ToastService } from '../../../core/services/toast.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent],
  template: `
    <div>
      <div class="page-top">
        <h1 class="heading-3">Products</h1>
        <div class="search-bar">
          <input type="text" class="input" placeholder="Search products..." [(ngModel)]="search" (input)="onSearch()" id="admin-search-products"/>
        </div>
      </div>

      @if (isLoading) {
        <app-loading-spinner />
      } @else if (errorMessage) {
        <div class="empty-state card">
          <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="#ef4444" stroke-width="1.5"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg>
          <p class="body-small text-muted" style="margin-top: var(--space-4);">{{ errorMessage }}</p>
          <button class="btn btn-ghost btn-sm" style="margin-top: var(--space-4);" (click)="loadProducts()">Retry</button>
        </div>
      } @else if (products.length === 0) {
        <div class="empty-state card">
          <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" style="color: var(--color-shade-50);"><path d="M6 2L3 6v14a2 2 0 002 2h14a2 2 0 002-2V6l-3-4z"/><line x1="3" y1="6" x2="21" y2="6"/><path d="M16 10a4 4 0 01-8 0"/></svg>
          <p class="body-small text-muted" style="margin-top: var(--space-4);">No products found</p>
        </div>
      } @else {
        <div class="table-card card">
          <table class="admin-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Category</th>
                <th>Price</th>
                <th>Stock</th>
                <th>Rating</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (product of products; track product.id) {
                <tr>
                  <td class="body-small">{{ product.name }}</td>
                  <td class="caption text-muted">{{ product.category?.name ?? '—' }}</td>
                  <td class="body-small">EGP {{ product.price | number:'1.2-2' }}</td>
                  <td class="caption" [class.text-neon]="product.stock > 0" [style.color]="product.stock <= 0 ? '#ef4444' : ''">
                    {{ product.stock }}
                    @if (product.stock <= 0) {
                      <span class="out-of-stock-badge">Out of stock</span>
                    }
                  </td>
                  <td class="caption text-muted">
                    @if (product.averageRating > 0) {
                      {{ product.averageRating | number:'1.1-1' }} ⭐ ({{ product.reviewCount }})
                    } @else {
                      —
                    }
                  </td>
                  <td>
                    <button class="btn btn-danger btn-sm" (click)="deleteProduct(product.id)" [id]="'delete-product-' + product.id">Delete</button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>

        <div class="pagination" style="margin-top: var(--space-7);">
          <button class="btn btn-ghost btn-sm" [disabled]="page <= 1" (click)="goToPage(page - 1)">← Previous</button>
          <span class="text-muted caption">Page {{ page }} of {{ totalPages }} · {{ totalItems }} products</span>
          <button class="btn btn-ghost btn-sm" [disabled]="page >= totalPages" (click)="goToPage(page + 1)">Next →</button>
        </div>
      }
    </div>
  `,
  styles: [`
    .page-top { display: flex; justify-content: space-between; align-items: center; gap: var(--space-4); margin-bottom: var(--space-7); flex-wrap: wrap; }
    .search-bar { width: 320px; }
    .table-card { overflow-x: auto; }
    .admin-table { width: 100%; border-collapse: collapse; }
    .admin-table th { text-align: left; padding: var(--space-4) var(--space-5); font-size: 12px; font-weight: 500; color: var(--color-muted); text-transform: uppercase; letter-spacing: 0.72px; border-bottom: 1px solid var(--color-shade-70); }
    .admin-table td { padding: var(--space-4) var(--space-5); border-bottom: 1px solid var(--color-dark-card-border); vertical-align: middle; }
    .admin-table tr:hover td { background: rgba(255,255,255,0.02); }
    .pagination { display: flex; align-items: center; justify-content: center; gap: var(--space-5); }
    .empty-state { display: flex; flex-direction: column; align-items: center; justify-content: center; padding: var(--space-10) var(--space-5); text-align: center; }
    .out-of-stock-badge { display: inline-block; font-size: 10px; font-weight: 600; padding: 2px 6px; border-radius: var(--radius-pill); background: rgba(239, 68, 68, 0.1); color: #ef4444; margin-left: 6px; }

    @media (max-width: 640px) {
      .search-bar { width: 100%; }
    }
  `]
})
export class AdminProductsComponent implements OnInit {
  private adminService = inject(AdminService);
  private toast = inject(ToastService);

  products: any[] = [];
  isLoading = true;
  errorMessage = '';
  search = '';
  page = 1;
  limit = 20;
  totalItems = 0;
  totalPages = 1;
  private debounceTimer: any;

  ngOnInit() { this.loadProducts(); }

  onSearch() {
    clearTimeout(this.debounceTimer);
    this.debounceTimer = setTimeout(() => { this.page = 1; this.loadProducts(); }, 400);
  }

  loadProducts() {
    this.isLoading = true;
    this.errorMessage = '';
    this.adminService.getProducts({ search: this.search || undefined, page: this.page, limit: this.limit })
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res) => {
          const data = res.data;
          if (data) {
            this.products = data.products ?? [];
            this.totalItems = data.totalItems ?? 0;
            this.totalPages = data.totalPages ?? 1;
          } else {
            this.products = [];
          }
        },
        error: () => {
          this.errorMessage = 'Failed to load products. Please try again.';
          this.toast.error('Failed to load products');
        },
      });
  }

  goToPage(p: number) { this.page = p; this.loadProducts(); }

  deleteProduct(id: string) {
    if (!confirm('Are you sure you want to delete this product?')) return;
    this.adminService.deleteProduct(id).subscribe({
      next: () => { this.toast.success('Product deleted'); this.loadProducts(); },
      error: () => this.toast.error('Failed to delete product'),
    });
  }
}
