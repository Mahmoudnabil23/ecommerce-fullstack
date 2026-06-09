import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { finalize } from 'rxjs';
import { CartService } from '../../core/services/cart.service';
import { ToastService } from '../../core/services/toast.service';
import { CartResponse, CartItem } from '../../core/models/cart.model';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule, LoadingSpinnerComponent],
  template: `
    <div class="section">
      <div class="container">
        <h1 class="heading-2" style="margin-bottom: var(--space-9);">Shopping Cart</h1>

        @if (isLoading) {
          <app-loading-spinner />
        } @else if (!cart || cart.items.length === 0) {
          <div class="empty-cart">
            <svg width="80" height="80" viewBox="0 0 24 24" fill="none" stroke="var(--color-shade-50)" stroke-width="1.5">
              <circle cx="9" cy="21" r="1"/><circle cx="20" cy="21" r="1"/>
              <path d="M1 1h4l2.68 13.39a2 2 0 002 1.61h9.72a2 2 0 002-1.61L23 6H6"/>
            </svg>
            <h3 class="heading-4">Your cart is empty</h3>
            <p class="text-muted">Looks like you haven't added anything yet.</p>
            <a routerLink="/products" class="btn btn-primary btn-lg" id="cart-shop-now">Start Shopping</a>
          </div>
        } @else {
          <div class="cart-layout">
            <!-- Cart Items -->
            <div class="cart-items">
              @for (item of cart.items; track item.cartItemId) {
                <div class="cart-item card" [id]="'cart-item-' + item.cartItemId">
                  <div class="cart-item-image">
                    @if (item.product.imageUrl) {
                      <img [src]="item.product.imageUrl" [alt]="item.product.name"/>
                    } @else {
                      <div class="skeleton" style="width: 100%; height: 100%;"></div>
                    }
                  </div>

                  <div class="cart-item-info">
                    <h3 class="body-medium">{{ item.product.name }}</h3>
                    <p class="text-muted caption">EGP {{ item.product.price | number:'1.2-2' }} each</p>
                  </div>

                  <div class="cart-item-qty">
                    <button class="qty-btn" (click)="updateQuantity(item, item.quantity - 1)" [disabled]="item.quantity <= 1">−</button>
                    <span class="qty-value">{{ item.quantity }}</span>
                    <button class="qty-btn" (click)="updateQuantity(item, item.quantity + 1)">+</button>
                  </div>

                  <div class="cart-item-subtotal">
                    <span class="body-medium">EGP {{ item.subtotal | number:'1.2-2' }}</span>
                  </div>

                  <button class="cart-item-remove" (click)="removeItem(item)" [id]="'remove-' + item.cartItemId" aria-label="Remove item">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M3 6h18M19 6v14a2 2 0 01-2 2H7a2 2 0 01-2-2V6m3 0V4a2 2 0 012-2h4a2 2 0 012 2v2"/></svg>
                  </button>
                </div>
              }
            </div>

            <!-- Cart Summary -->
            <div class="cart-summary card">
              <h3 class="heading-5" style="margin-bottom: var(--space-5);">Order Summary</h3>

              <div class="summary-rows">
                <div class="summary-row">
                  <span class="text-muted">Subtotal</span>
                  <span>EGP {{ cart.summary.subtotal | number:'1.2-2' }}</span>
                </div>
                @if (cart.summary.discount > 0) {
                  <div class="summary-row">
                    <span class="text-muted">Discount</span>
                    <span class="text-neon">-EGP {{ cart.summary.discount | number:'1.2-2' }}</span>
                  </div>
                }
                <div class="summary-row">
                  <span class="text-muted">Shipping</span>
                  <span>{{ cart.summary.shipping > 0 ? 'EGP ' + (cart.summary.shipping | number:'1.2-2') : 'Free' }}</span>
                </div>
                <div class="summary-divider"></div>
                <div class="summary-row summary-total">
                  <span class="heading-5">Total</span>
                  <span class="heading-5">EGP {{ cart.summary.total | number:'1.2-2' }}</span>
                </div>
              </div>

              <a routerLink="/checkout" class="btn btn-primary btn-lg w-full" id="proceed-checkout" style="margin-top: var(--space-5);">Proceed to Checkout</a>
              <a routerLink="/products" class="btn btn-ghost btn-sm w-full" style="margin-top: var(--space-3);">Continue Shopping</a>
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .empty-cart {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-5);
      padding: 100px 0;
      text-align: center;
    }

    .cart-layout {
      display: grid;
      grid-template-columns: 1fr 380px;
      gap: var(--space-7);
      align-items: start;
    }

    .cart-items {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
    }

    .cart-item {
      display: flex;
      align-items: center;
      gap: var(--space-5);
      padding: var(--space-4);
    }

    .cart-item-image {
      width: 80px;
      height: 80px;
      border-radius: var(--radius-sm);
      overflow: hidden;
      flex-shrink: 0;
      background: var(--color-dark-forest);
    }

    .cart-item-image img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .cart-item-info {
      flex: 1;
      min-width: 0;
    }

    .cart-item-info h3 {
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .cart-item-qty {
      display: flex;
      align-items: center;
      border: 1px solid var(--color-shade-70);
      border-radius: var(--radius-sm);
      flex-shrink: 0;
    }

    .qty-btn {
      width: 36px;
      height: 36px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: var(--color-white);
      font-size: 18px;
      transition: background var(--transition-fast);
    }

    .qty-btn:hover:not(:disabled) { background: var(--color-dark-forest); }
    .qty-btn:disabled { color: var(--color-shade-60); cursor: not-allowed; }

    .qty-value {
      width: 40px;
      text-align: center;
      font-weight: 500;
      border-left: 1px solid var(--color-shade-70);
      border-right: 1px solid var(--color-shade-70);
      padding: 6px 0;
    }

    .cart-item-subtotal {
      flex-shrink: 0;
      min-width: 100px;
      text-align: right;
    }

    .cart-item-remove {
      color: var(--color-shade-50);
      padding: 8px;
      transition: color var(--transition-fast);
      flex-shrink: 0;
    }

    .cart-item-remove:hover { color: #ef4444; }

    .cart-summary {
      padding: var(--space-5);
      position: sticky;
      top: 96px;
    }

    .summary-rows {
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
    }

    .summary-row {
      display: flex;
      justify-content: space-between;
      font-size: 15px;
    }

    .summary-divider {
      border-top: 1px solid var(--color-shade-70);
      margin: var(--space-3) 0;
    }

    .summary-total {
      font-weight: 600;
    }

    @media (max-width: 1024px) {
      .cart-layout {
        grid-template-columns: 1fr;
      }

      .cart-summary {
        position: static;
      }
    }

    @media (max-width: 640px) {
      .cart-item {
        flex-wrap: wrap;
        gap: var(--space-3);
      }

      .cart-item-subtotal {
        min-width: auto;
      }
    }
  `]
})
export class CartComponent implements OnInit {
  private cartService = inject(CartService);
  private toast = inject(ToastService);

  cart: CartResponse | null = null;
  isLoading = true;

  ngOnInit() {
    this.isLoading = true;
    this.cartService.cart$.subscribe((cart) => {
      this.cart = cart;
    });
    this.cartService.loadCart()
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        error: () => this.toast.error('Failed to load cart'),
      });
  }

  updateQuantity(item: CartItem, newQty: number) {
    if (newQty < 1) return;
    this.cartService.updateItem(item.cartItemId, { quantity: newQty }).subscribe({
      error: () => this.toast.error('Failed to update quantity'),
    });
  }

  removeItem(item: CartItem) {
    this.cartService.removeItem(item.cartItemId).subscribe({
      next: () => this.toast.success('Item removed'),
      error: () => this.toast.error('Failed to remove item'),
    });
  }
}
