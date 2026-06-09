import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { finalize } from 'rxjs';
import { ProductService } from '../../../core/services/product.service';
import { CartService } from '../../../core/services/cart.service';
import { ToastService } from '../../../core/services/toast.service';
import { ProductListItem, ProductFilter } from '../../../core/models/product.model';
import { ProductCardComponent } from '../../../shared/components/product-card/product-card.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, ProductCardComponent, LoadingSpinnerComponent],
  template: `
    <div class="section">
      <div class="container">
        <div class="page-header">
          <h1 class="heading-2">All Products</h1>
          <p class="text-muted body-large">Browse our complete collection</p>
        </div>

        <div class="product-list-layout">
          <!-- Filters Sidebar -->
          <aside class="filters-sidebar card">
            <h3 class="heading-6">Filters</h3>

            <div class="filter-group">
              <label class="form-label">Search</label>
              <input
                type="text"
                class="input"
                placeholder="Search products..."
                [(ngModel)]="filter.search"
                (input)="onFilterChange()"
                id="filter-search"/>
            </div>

            <div class="filter-group">
              <label class="form-label">Min Price (EGP)</label>
              <input
                type="number"
                class="input"
                placeholder="0"
                [(ngModel)]="filter.minPrice"
                (change)="onFilterChange()"
                id="filter-min-price"/>
            </div>

            <div class="filter-group">
              <label class="form-label">Max Price (EGP)</label>
              <input
                type="number"
                class="input"
                placeholder="Any"
                [(ngModel)]="filter.maxPrice"
                (change)="onFilterChange()"
                id="filter-max-price"/>
            </div>

            <div class="filter-group">
              <label class="form-label">Sort By</label>
              <select class="input" [(ngModel)]="filter.sortBy" (change)="onFilterChange()" id="filter-sort">
                <option value="">Default</option>
                <option value="newest">Newest</option>
                <option value="price_asc">Price: Low → High</option>
                <option value="price_desc">Price: High → Low</option>
                <option value="rating">Top Rated</option>
              </select>
            </div>

            <div class="filter-group">
              <label class="filter-checkbox">
                <input type="checkbox" [(ngModel)]="filter.inStock" (change)="onFilterChange()" id="filter-in-stock"/>
                <span>In Stock Only</span>
              </label>
            </div>

            <button class="btn btn-ghost btn-sm w-full" (click)="clearFilters()" id="filter-clear">Clear Filters</button>
          </aside>

          <!-- Products Grid -->
          <div class="products-section">
            @if (isLoading) {
              <app-loading-spinner />
            } @else if (products.length === 0) {
              <div class="empty-state">
                <svg width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="var(--color-shade-50)" stroke-width="1.5">
                  <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
                </svg>
                <p class="heading-5">No products found</p>
                <p class="text-muted">Try adjusting your filters or search terms</p>
              </div>
            } @else {
              <div class="products-grid grid grid-3">
                @for (product of products; track product.id) {
                  <app-product-card [product]="product" (addToCart)="onAddToCart($event)" />
                }
              </div>

              <!-- Pagination -->
              <div class="pagination">
                <button class="btn btn-ghost btn-sm" [disabled]="filter.page === 1" (click)="goToPage((filter.page ?? 1) - 1)" id="page-prev">← Previous</button>
                <span class="text-muted caption">Page {{ filter.page }}</span>
                <button class="btn btn-ghost btn-sm" [disabled]="products.length < (filter.limit ?? 20)" (click)="goToPage((filter.page ?? 1) + 1)" id="page-next">Next →</button>
              </div>
            }
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-header {
      margin-bottom: var(--space-9);
    }

    .page-header .heading-2 {
      margin-bottom: var(--space-2);
    }

    .product-list-layout {
      display: grid;
      grid-template-columns: 280px 1fr;
      gap: var(--space-7);
      align-items: start;
    }

    .filters-sidebar {
      padding: var(--space-5);
      display: flex;
      flex-direction: column;
      gap: var(--space-5);
      position: sticky;
      top: 96px;
    }

    .filter-group {
      display: flex;
      flex-direction: column;
      gap: var(--space-2);
    }

    .filter-checkbox {
      display: flex;
      align-items: center;
      gap: var(--space-3);
      cursor: pointer;
      color: var(--color-shade-30);
      font-size: 14px;
    }

    .filter-checkbox input[type="checkbox"] {
      width: 18px;
      height: 18px;
      accent-color: var(--color-neon-green);
    }

    .products-section {
      min-height: 400px;
    }

    .products-grid {
      gap: var(--space-5);
    }

    .pagination {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: var(--space-5);
      margin-top: var(--space-9);
    }

    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: var(--space-4);
      padding: 120px var(--space-5);
      text-align: center;
    }

    @media (max-width: 1024px) {
      .product-list-layout {
        grid-template-columns: 1fr;
      }

      .filters-sidebar {
        position: static;
        flex-direction: row;
        flex-wrap: wrap;
      }

      .filters-sidebar .heading-6 {
        width: 100%;
      }

      .filter-group {
        flex: 1;
        min-width: 160px;
      }
    }

    @media (max-width: 768px) {
      .products-grid {
        grid-template-columns: repeat(2, 1fr) !important;
      }
    }

    @media (max-width: 480px) {
      .products-grid {
        grid-template-columns: 1fr !important;
      }
    }
  `]
})
export class ProductListComponent implements OnInit {
  private productService = inject(ProductService);
  private cartService = inject(CartService);
  private toast = inject(ToastService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private authService = inject(AuthService);

  products: ProductListItem[] = [];
  isLoading = true;
  filter: ProductFilter = { page: 1, limit: 12 };

  private debounceTimer: any;

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      if (params['sortBy']) this.filter.sortBy = params['sortBy'];
      if (params['search']) this.filter.search = params['search'];
      if (params['categoryId']) this.filter.categoryId = params['categoryId'];
      this.loadProducts();
    });
  }

  onFilterChange() {
    clearTimeout(this.debounceTimer);
    this.debounceTimer = setTimeout(() => {
      this.filter.page = 1;
      this.loadProducts();
    }, 400);
  }

  loadProducts() {
    this.isLoading = true;
    this.productService.getProducts(this.filter)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res) => {
          if (res.success) {
            this.products = res.data?.items ?? [];
          } else {
            this.toast.error(res.message || 'Failed to load products');
          }
        },
        error: () => {
          this.toast.error('Failed to load products');
        },
      });
  }

  goToPage(page: number) {
    this.filter.page = page;
    this.loadProducts();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  clearFilters() {
    this.filter = { page: 1, limit: 12 };
    this.loadProducts();
  }

  onAddToCart(product: ProductListItem) {
    if (!this.authService.isLoggedIn) {
      this.toast.info('Please sign in first to add items to cart.');
      this.router.navigate(['/login'], { queryParams: { returnUrl: this.router.url } });
      return;
    }

    this.cartService.addItem({ productId: product.id, quantity: 1 }).subscribe({
      next: () => this.toast.success(`${product.name} added to cart`),
      error: (error) => {
        if (error?.status === 401) {
          this.toast.info('Your session expired. Please sign in again.');
          this.router.navigate(['/login'], { queryParams: { returnUrl: this.router.url } });
          return;
        }

        this.toast.error(error?.error?.message || 'Failed to add item to cart');
      },
    });
  }
}
