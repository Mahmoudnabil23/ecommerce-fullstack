import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductListItem } from '../../../core/models/product.model';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <a [routerLink]="['/products', product.id]" class="product-card card" [id]="'product-card-' + product.id">
      <div class="product-image-wrapper">
        @if (product.images && product.images.length > 0) {
          <img [src]="product.images[0]" [alt]="product.name" class="product-image" loading="lazy"/>
        } @else {
          <div class="product-image-placeholder skeleton"></div>
        }
        @if (product.discountedPrice && product.discountedPrice < product.price) {
          <span class="product-badge">
            -{{ getDiscountPercent() }}%
          </span>
        }
      </div>

      <div class="product-info">
        <p class="product-category caption text-muted">{{ product.category.name }}</p>
        <h3 class="product-name">{{ product.name }}</h3>

        <div class="product-rating">
          @for (star of stars; track star) {
            <svg width="14" height="14" viewBox="0 0 24 24" [class.filled]="star <= product.averageRating" class="star-icon">
              <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z" fill="currentColor"/>
            </svg>
          }
          <span class="review-count text-shade-50">({{ product.reviewCount }})</span>
        </div>

        <div class="product-pricing">
          @if (product.discountedPrice && product.discountedPrice < product.price) {
            <span class="price-current">EGP {{ product.discountedPrice | number:'1.2-2' }}</span>
            <span class="price-original text-shade-50">EGP {{ product.price | number:'1.2-2' }}</span>
          } @else {
            <span class="price-current">EGP {{ product.price | number:'1.2-2' }}</span>
          }
        </div>

        @if (product.stock <= 0) {
          <span class="out-of-stock caption">Out of Stock</span>
        }
      </div>

      <button
        class="add-to-cart-btn"
        (click)="onAddToCart($event)"
        [disabled]="product.stock <= 0"
        [id]="'add-to-cart-' + product.id">
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <path d="M12 5v14M5 12h14"/>
        </svg>
      </button>
    </a>
  `,
  styles: [`
    .product-card {
      display: flex;
      flex-direction: column;
      overflow: hidden;
      text-decoration: none;
      color: var(--color-white);
      position: relative;
      cursor: pointer;
    }

    .product-image-wrapper {
      position: relative;
      aspect-ratio: 1;
      overflow: hidden;
      background: var(--color-dark-forest);
    }

    .product-image {
      width: 100%;
      height: 100%;
      object-fit: cover;
      transition: transform var(--transition-normal);
    }

    .product-card:hover .product-image {
      transform: scale(1.05);
    }

    .product-image-placeholder {
      width: 100%;
      height: 100%;
    }

    .product-badge {
      position: absolute;
      top: var(--space-3);
      left: var(--space-3);
      background: var(--color-neon-green);
      color: var(--color-black);
      font-size: 12px;
      font-weight: 600;
      padding: 4px 10px;
      border-radius: var(--radius-pill);
    }

    .product-info {
      padding: var(--space-4);
      display: flex;
      flex-direction: column;
      gap: var(--space-2);
      flex: 1;
    }

    .product-category {
      text-transform: uppercase;
      letter-spacing: 0.72px;
    }

    .product-name {
      font-family: var(--font-display);
      font-size: 16px;
      font-weight: 500;
      line-height: 1.3;
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
    }

    .product-rating {
      display: flex;
      align-items: center;
      gap: 2px;
    }

    .star-icon {
      color: var(--color-shade-70);
    }

    .star-icon.filled {
      color: #FBBF24;
    }

    .review-count {
      font-size: 12px;
      margin-left: 4px;
    }

    .product-pricing {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      margin-top: auto;
    }

    .price-current {
      font-family: var(--font-display);
      font-size: 18px;
      font-weight: 600;
      color: var(--color-white);
    }

    .price-original {
      font-size: 14px;
      text-decoration: line-through;
    }

    .out-of-stock {
      color: #ef4444;
      font-weight: 600;
    }

    .add-to-cart-btn {
      position: absolute;
      bottom: var(--space-4);
      right: var(--space-4);
      width: 40px;
      height: 40px;
      border-radius: 50%;
      background: var(--color-white);
      color: var(--color-black);
      display: flex;
      align-items: center;
      justify-content: center;
      opacity: 0;
      transform: translateY(8px);
      transition: all var(--transition-fast);
      border: none;
      cursor: pointer;
    }

    .product-card:hover .add-to-cart-btn {
      opacity: 1;
      transform: translateY(0);
    }

    .add-to-cart-btn:hover {
      background: var(--color-neon-green);
    }

    .add-to-cart-btn:disabled {
      opacity: 0.3;
      cursor: not-allowed;
    }
  `]
})
export class ProductCardComponent {
  @Input({ required: true }) product!: ProductListItem;
  @Output() addToCart = new EventEmitter<ProductListItem>();

  stars = [1, 2, 3, 4, 5];

  getDiscountPercent(): number {
    if (!this.product.discountedPrice) return 0;
    return Math.round((1 - this.product.discountedPrice / this.product.price) * 100);
  }

  onAddToCart(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.addToCart.emit(this.product);
  }
}
