import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { finalize } from 'rxjs';
import { ProductService } from '../../../core/services/product.service';
import { CartService } from '../../../core/services/cart.service';
import { ToastService } from '../../../core/services/toast.service';
import { ProductDetail } from '../../../core/models/product.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, LoadingSpinnerComponent],
  template: `
    @if (isLoading) {
      <app-loading-spinner />
    } @else if (product) {
      <div class="section">
        <div class="container">
          <!-- Breadcrumb -->
          <nav class="breadcrumb caption text-muted">
            <a routerLink="/">Home</a> /
            <a routerLink="/products">Products</a> /
            <span class="text-white">{{ product.name }}</span>
          </nav>

          <div class="product-detail-layout">
            <!-- Image Gallery -->
            <div class="product-gallery">
              <div class="gallery-main card">
                @if (product.images && product.images.length > 0) {
                  <img [src]="product.images[selectedImageIndex]" [alt]="product.name" class="gallery-main-img"/>
                } @else {
                  <div class="gallery-placeholder skeleton" style="aspect-ratio: 1;"></div>
                }
              </div>
              @if (product.images && product.images.length > 1) {
                <div class="gallery-thumbs">
                  @for (img of product.images; track img; let i = $index) {
                    <button
                      class="gallery-thumb"
                      [class.active]="i === selectedImageIndex"
                      (click)="selectedImageIndex = i"
                      [id]="'thumb-' + i">
                      <img [src]="img" [alt]="product.name + ' image ' + (i + 1)"/>
                    </button>
                  }
                </div>
              }
            </div>

            <!-- Product Info -->
            <div class="product-info">
              <span class="overline text-muted">{{ product.category.name }}</span>
              <h1 class="heading-3">{{ product.name }}</h1>

              <!-- Rating -->
              <div class="product-rating">
                @for (star of [1,2,3,4,5]; track star) {
                  <svg width="18" height="18" viewBox="0 0 24 24" [class.filled]="star <= product.averageRating" class="star-icon">
                    <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z" fill="currentColor"/>
                  </svg>
                }
                <span class="text-muted caption">{{ product.averageRating | number:'1.1-1' }} ({{ product.reviewCount }} reviews)</span>
              </div>

              <!-- Price -->
              <div class="product-price-section">
                @if (product.discountedPrice && product.discountedPrice < product.price) {
                  <span class="price-discount heading-4">EGP {{ product.discountedPrice | number:'1.2-2' }}</span>
                  <span class="price-original text-shade-50 heading-6" style="text-decoration: line-through;">EGP {{ product.price | number:'1.2-2' }}</span>
                  <span class="discount-badge">-{{ getDiscountPercent() }}%</span>
                } @else {
                  <span class="heading-4">EGP {{ product.price | number:'1.2-2' }}</span>
                }
              </div>

              <!-- Stock -->
              <div class="product-stock caption">
                @if (product.stock > 0) {
                  <span class="stock-available">
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="#36F4A4" stroke-width="2"><path d="M20 6L9 17l-5-5"/></svg>
                    In Stock ({{ product.stock }} available)
                  </span>
                } @else {
                  <span class="stock-out" style="color: #ef4444;">Out of Stock</span>
                }
              </div>

              <!-- Description -->
              <div class="product-description">
                <h3 class="heading-6" style="margin-bottom: var(--space-3);">Description</h3>
                <p class="body text-shade-30">{{ product.description }}</p>
              </div>

              <!-- Quantity & Add to Cart -->
              <div class="product-actions">
                <div class="quantity-control">
                  <button class="qty-btn" (click)="decreaseQty()" [disabled]="quantity <= 1" id="qty-decrease">−</button>
                  <span class="qty-value">{{ quantity }}</span>
                  <button class="qty-btn" (click)="increaseQty()" [disabled]="quantity >= product.stock" id="qty-increase">+</button>
                </div>
                <button
                  class="btn btn-primary btn-lg flex-1"
                  [disabled]="product.stock <= 0"
                  (click)="addToCart()"
                  id="add-to-cart">
                  Add to Cart — EGP {{ getTotalPrice() | number:'1.2-2' }}
                </button>
              </div>

              <!-- Seller -->
              @if (product.seller) {
                <div class="product-seller caption text-muted">
                  Sold by <span class="text-white">{{ product.seller.storeName }}</span>
                </div>
              }
            </div>
          </div>
        </div>
      </div>
    } @else {
      <div class="section">
        <div class="container text-center">
          <h2 class="heading-3">Product not found</h2>
          <p class="text-muted" style="margin: var(--space-4) 0;">The product you're looking for doesn't exist.</p>
          <a routerLink="/products" class="btn btn-primary">Browse Products</a>
        </div>
      </div>
    }
  `,
  styles: [`
    .breadcrumb {
      margin-bottom: var(--space-7);
    }

    .breadcrumb a {
      color: var(--color-muted);
    }

    .breadcrumb a:hover {
      color: var(--color-white);
    }

    .product-detail-layout {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-10);
      align-items: start;
    }

    .gallery-main {
      overflow: hidden;
      border-radius: var(--radius-md);
    }

    .gallery-main-img {
      width: 100%;
      aspect-ratio: 1;
      object-fit: cover;
    }

    .gallery-thumbs {
      display: flex;
      gap: var(--space-3);
      margin-top: var(--space-4);
    }

    .gallery-thumb {
      width: 72px;
      height: 72px;
      border-radius: var(--radius-sm);
      overflow: hidden;
      border: 2px solid transparent;
      opacity: 0.6;
      transition: all var(--transition-fast);
      cursor: pointer;
      padding: 0;
    }

    .gallery-thumb.active {
      border-color: var(--color-neon-green);
      opacity: 1;
    }

    .gallery-thumb:hover {
      opacity: 1;
    }

    .gallery-thumb img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .product-info {
      display: flex;
      flex-direction: column;
      gap: var(--space-5);
    }

    .product-rating {
      display: flex;
      align-items: center;
      gap: 3px;
    }

    .star-icon { color: var(--color-shade-70); }
    .star-icon.filled { color: #FBBF24; }

    .product-rating .caption { margin-left: var(--space-2); }

    .product-price-section {
      display: flex;
      align-items: center;
      gap: var(--space-4);
    }

    .price-discount {
      color: var(--color-neon-green);
    }

    .discount-badge {
      background: rgba(54, 244, 164, 0.15);
      color: var(--color-neon-green);
      font-size: 13px;
      font-weight: 600;
      padding: 4px 10px;
      border-radius: var(--radius-pill);
    }

    .product-stock {
      display: flex;
      align-items: center;
      gap: var(--space-2);
    }

    .stock-available {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      color: var(--color-neon-green);
    }

    .product-actions {
      display: flex;
      gap: var(--space-4);
      align-items: center;
    }

    .flex-1 { flex: 1; }

    .quantity-control {
      display: flex;
      align-items: center;
      border: 1px solid var(--color-shade-70);
      border-radius: var(--radius-sm);
    }

    .qty-btn {
      width: 44px;
      height: 44px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: var(--color-white);
      font-size: 20px;
      transition: background var(--transition-fast);
    }

    .qty-btn:hover:not(:disabled) {
      background: var(--color-dark-forest);
    }

    .qty-btn:disabled {
      color: var(--color-shade-60);
      cursor: not-allowed;
    }

    .qty-value {
      width: 48px;
      text-align: center;
      font-weight: 500;
      font-size: 16px;
      border-left: 1px solid var(--color-shade-70);
      border-right: 1px solid var(--color-shade-70);
      padding: 10px 0;
    }

    .product-seller {
      padding-top: var(--space-4);
      border-top: 1px solid var(--color-shade-70);
    }

    @media (max-width: 1024px) {
      .product-detail-layout {
        grid-template-columns: 1fr;
        gap: var(--space-7);
      }
    }
  `]
})
export class ProductDetailComponent implements OnInit {
  private productService = inject(ProductService);
  private cartService = inject(CartService);
  private toast = inject(ToastService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private authService = inject(AuthService);

  product: ProductDetail | null = null;
  isLoading = true;
  selectedImageIndex = 0;
  quantity = 1;

  ngOnInit() {
    this.route.params.subscribe((params) => {
      const id = params['id'];
      if (id) {
        this.loadProduct(id);
      }
    });
  }

  loadProduct(id: string) {
    this.isLoading = true;
    this.productService.getProductById(id)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res) => {
          this.product = (res.data as any) ?? null;
        },
        error: () => {
          this.toast.error('Failed to load product');
        },
      });
  }

  getDiscountPercent(): number {
    if (!this.product?.discountedPrice) return 0;
    return Math.round((1 - this.product.discountedPrice / this.product.price) * 100);
  }

  getTotalPrice(): number {
    if (!this.product) return 0;
    const price = this.product.discountedPrice ?? this.product.price;
    return price * this.quantity;
  }

  increaseQty() {
    if (this.product && this.quantity < this.product.stock) this.quantity++;
  }

  decreaseQty() {
    if (this.quantity > 1) this.quantity--;
  }

  addToCart() {
    if (!this.product) return;

    if (!this.authService.isLoggedIn) {
      this.toast.info('Please sign in first to add items to cart.');
      this.router.navigate(['/login'], { queryParams: { returnUrl: this.router.url } });
      return;
    }

    this.cartService.addItem({ productId: this.product.id, quantity: this.quantity }).subscribe({
      next: () => this.toast.success(`${this.product!.name} added to cart`),
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
