import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { finalize } from 'rxjs';
import { ProductService } from '../../core/services/product.service';
import { CartService } from '../../core/services/cart.service';
import { ToastService } from '../../core/services/toast.service';
import { ProductListItem } from '../../core/models/product.model';
import { ProductCardComponent } from '../../shared/components/product-card/product-card.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule, ProductCardComponent, LoadingSpinnerComponent],
  template: `
    <!-- Hero Section -->
    <section class="hero">
      <div class="hero-bg"></div>
      <div class="container hero-content">
        <span class="overline text-muted animate-fade-in">Premium E-Commerce Experience</span>
        <h1 class="display-light animate-fade-in" style="animation-delay: 0.1s">
          Discover products<br/>you'll <span class="text-neon">love</span>
        </h1>
        <p class="body-large text-muted hero-subtitle animate-fade-in" style="animation-delay: 0.2s">
          Explore a curated collection of premium products, delivered to your door with unmatched quality and care.
        </p>
        <div class="hero-actions animate-fade-in" style="animation-delay: 0.3s">
          <a routerLink="/products" class="btn btn-primary btn-lg" id="hero-shop-now">Shop now</a>
          <a routerLink="/register" class="btn btn-ghost btn-lg" id="hero-get-started">Create account</a>
        </div>
      </div>
    </section>

    <!-- Stats Section -->
    <section class="section surface-dark-forest">
      <div class="container">
        <div class="stats-grid">
          <div class="stat-item animate-fade-in-up">
            <span class="stat-number display-xl-bold">500+</span>
            <span class="stat-label text-muted caption">Products Available</span>
          </div>
          <div class="stat-item animate-fade-in-up" style="animation-delay: 0.1s">
            <span class="stat-number display-xl-bold">10K+</span>
            <span class="stat-label text-muted caption">Happy Customers</span>
          </div>
          <div class="stat-item animate-fade-in-up" style="animation-delay: 0.2s">
            <span class="stat-number display-xl-bold">99%</span>
            <span class="stat-label text-muted caption">Satisfaction Rate</span>
          </div>
          <div class="stat-item animate-fade-in-up" style="animation-delay: 0.3s">
            <span class="stat-number display-xl-bold">24/7</span>
            <span class="stat-label text-muted caption">Customer Support</span>
          </div>
        </div>
      </div>
    </section>

    <!-- Featured Products -->
    <section class="section">
      <div class="container">
        <div class="section-header">
          <div>
            <span class="overline text-neon">Featured</span>
            <h2 class="heading-2">Top products</h2>
          </div>
          <a routerLink="/products" class="btn btn-ghost" id="view-all-products">View all →</a>
        </div>

        @if (isLoading) {
          <app-loading-spinner />
        } @else {
          <div class="products-grid grid grid-4">
            @for (product of featuredProducts; track product.id) {
              <app-product-card [product]="product" (addToCart)="onAddToCart($event)" />
            }
          </div>
        }
      </div>
    </section>

    <!-- CTA Section -->
    <section class="section surface-deep-teal">
      <div class="container cta-section">
        <div class="cta-content">
          <h2 class="heading-2">Ready to start shopping?</h2>
          <p class="body-large text-muted">Join thousands of satisfied customers and discover products you'll love.</p>
          <div class="hero-actions">
            <a routerLink="/register" class="btn btn-primary btn-lg" id="cta-register">Create free account</a>
            <a routerLink="/products" class="btn btn-ghost btn-lg" id="cta-browse">Browse products</a>
          </div>
        </div>
      </div>
    </section>
  `,
  styles: [`
    /* Hero */
    .hero {
      position: relative;
      min-height: 90vh;
      display: flex;
      align-items: center;
      overflow: hidden;
    }

    .hero-bg {
      position: absolute;
      inset: 0;
      background: radial-gradient(ellipse at 50% 30%, var(--color-forest) 0%, var(--color-deep-teal) 40%, var(--color-void) 70%);
      z-index: 0;
    }

    .hero-content {
      position: relative;
      z-index: 1;
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      gap: var(--space-5);
      padding-top: var(--space-10);
      padding-bottom: var(--space-10);
    }

    .hero-subtitle {
      max-width: 560px;
    }

    .hero-actions {
      display: flex;
      gap: var(--space-4);
      flex-wrap: wrap;
      justify-content: center;
    }

    /* Stats */
    .stats-grid {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: var(--space-7);
      text-align: center;
    }

    .stat-item {
      display: flex;
      flex-direction: column;
      gap: var(--space-2);
    }

    .stat-number {
      font-size: 56px;
      font-weight: 700;
      line-height: 1;
    }

    /* Section Header */
    .section-header {
      display: flex;
      align-items: flex-end;
      justify-content: space-between;
      margin-bottom: var(--space-9);
      gap: var(--space-4);
    }

    .section-header .overline {
      margin-bottom: var(--space-2);
      display: block;
    }

    /* Products Grid */
    .products-grid {
      gap: var(--space-5);
    }

    /* CTA */
    .cta-section {
      text-align: center;
      max-width: 720px;
    }

    .cta-content {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-5);
    }

    @media (max-width: 1024px) {
      .stats-grid { grid-template-columns: repeat(2, 1fr); }
      .stat-number { font-size: 40px; }
    }

    @media (max-width: 768px) {
      .hero { min-height: 70vh; }
      .section-header { flex-direction: column; align-items: flex-start; }
    }

    @media (max-width: 640px) {
      .stats-grid { grid-template-columns: 1fr 1fr; gap: var(--space-5); }
      .stat-number { font-size: 32px; }
    }
  `]
})
export class HomeComponent implements OnInit {
  private productService = inject(ProductService);
  private cartService = inject(CartService);
  private toast = inject(ToastService);
  private authService = inject(AuthService);
  private router = inject(Router);

  featuredProducts: ProductListItem[] = [];
  isLoading = true;

  ngOnInit() {
    this.productService.getProducts({ limit: 8, sortBy: 'rating' })
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res) => {
          this.featuredProducts = res.data?.items ?? [];
        },
        error: () => {
          this.toast.error('Failed to load featured products');
        },
      });
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
