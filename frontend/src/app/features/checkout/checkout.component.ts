import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { finalize } from 'rxjs';
import { CartService } from '../../core/services/cart.service';
import { OrderService } from '../../core/services/order.service';
import { ToastService } from '../../core/services/toast.service';
import { PaymentService } from '../../core/services/payment.service';
import { CartResponse } from '../../core/models/cart.model';
import { PaymentMethod } from '../../core/models/order.model';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="section">
      <div class="container">
        <h1 class="heading-2" style="margin-bottom: var(--space-9);">Checkout</h1>

        @if (!cart || cart.items.length === 0) {
          <div class="empty-state">
            <p class="heading-5">Your cart is empty</p>
            <a routerLink="/products" class="btn btn-primary btn-lg">Continue Shopping</a>
          </div>
        } @else {
          <div class="checkout-layout">
            <!-- Shipping Info -->
            <div class="checkout-form">
              <div class="checkout-section card">
                <h2 class="heading-5" style="margin-bottom: var(--space-5);">Shipping Address</h2>
                <p class="text-muted body-small">
                  The backend currently places orders using your default saved address. Manage addresses in backend profile endpoints.
                </p>
              </div>

              <!-- Payment Method -->
              <div class="checkout-section card">
                <h2 class="heading-5" style="margin-bottom: var(--space-5);">Payment Method</h2>
                <div class="payment-methods">
                  <label class="payment-option" [class.selected]="paymentMethod === 'cod'" id="payment-cod">
                    <input type="radio" name="payment" value="cod" [(ngModel)]="paymentMethod"/>
                    <div class="payment-option-content">
                      <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="1" y="4" width="22" height="16" rx="2"/><line x1="1" y1="10" x2="23" y2="10"/></svg>
                      <div>
                        <span class="body-medium">Cash on Delivery</span>
                        <span class="caption text-muted">Pay when you receive your order</span>
                      </div>
                    </div>
                  </label>
                  <label class="payment-option" [class.selected]="paymentMethod === 'stripe'" id="payment-stripe">
                    <input type="radio" name="payment" value="stripe" [(ngModel)]="paymentMethod"/>
                    <div class="payment-option-content">
                      <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="1" y="4" width="22" height="16" rx="2"/><line x1="1" y1="10" x2="23" y2="10"/></svg>
                      <div>
                        <span class="body-medium">Credit / Debit Card</span>
                        <span class="caption text-muted">Secure payment via Stripe</span>
                      </div>
                    </div>
                  </label>
                </div>
              </div>

              @if (errorMessage) {
                <p class="form-error" style="margin-top: var(--space-3);">{{ errorMessage }}</p>
              }
            </div>

            <!-- Order Summary -->
            <div class="order-summary card">
              <h3 class="heading-5" style="margin-bottom: var(--space-5);">Order Summary</h3>

              <div class="summary-items">
                @for (item of cart.items; track item.cartItemId) {
                  <div class="summary-item">
                    <div class="summary-item-img">
                      @if (item.product.imageUrl) {
                        <img [src]="item.product.imageUrl" [alt]="item.product.name"/>
                      } @else {
                        <div class="skeleton" style="width:100%;height:100%;"></div>
                      }
                    </div>
                    <div class="summary-item-info">
                      <span class="body-small">{{ item.product.name }}</span>
                      <span class="caption text-muted">Qty: {{ item.quantity }}</span>
                    </div>
                    <span class="body-small">EGP {{ item.subtotal | number:'1.2-2' }}</span>
                  </div>
                }
              </div>

              <div class="summary-divider"></div>

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

              <button
                class="btn btn-primary btn-lg w-full"
                [disabled]="isSubmitting"
                (click)="placeOrder()"
                id="place-order"
                style="margin-top: var(--space-5);">
                @if (isSubmitting) {
                  <span class="btn-spinner"></span>
                }
                {{ isSubmitting ? 'Processing...' : 'Place Order' }}
              </button>
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-5);
      padding: 100px 0;
      text-align: center;
    }

    .checkout-layout {
      display: grid;
      grid-template-columns: 1fr 420px;
      gap: var(--space-7);
      align-items: start;
    }

    .checkout-form {
      display: flex;
      flex-direction: column;
      gap: var(--space-5);
    }

    .checkout-section {
      padding: var(--space-5);
    }

    .form-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-4);
    }

    .full-width {
      grid-column: 1 / -1;
    }

    .payment-methods {
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
    }

    .payment-option {
      cursor: pointer;
      border: 1px solid var(--color-shade-70);
      border-radius: var(--radius-sm);
      padding: var(--space-4);
      transition: all var(--transition-fast);
    }

    .payment-option:hover {
      border-color: var(--color-shade-50);
    }

    .payment-option.selected {
      border-color: var(--color-neon-green);
      background: rgba(54, 244, 164, 0.05);
    }

    .payment-option input[type="radio"] {
      display: none;
    }

    .payment-option-content {
      display: flex;
      align-items: center;
      gap: var(--space-4);
    }

    .payment-option-content div {
      display: flex;
      flex-direction: column;
      gap: 2px;
    }

    .order-summary {
      padding: var(--space-5);
      position: sticky;
      top: 96px;
    }

    .summary-items {
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
    }

    .summary-item {
      display: flex;
      align-items: center;
      gap: var(--space-3);
    }

    .summary-item-img {
      width: 48px;
      height: 48px;
      border-radius: var(--radius-xs);
      overflow: hidden;
      flex-shrink: 0;
      background: var(--color-dark-forest);
    }

    .summary-item-img img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .summary-item-info {
      flex: 1;
      display: flex;
      flex-direction: column;
      min-width: 0;
    }

    .summary-item-info .body-small {
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
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
      margin: var(--space-4) 0;
    }

    .summary-total { font-weight: 600; }

    .btn-spinner {
      width: 18px;
      height: 18px;
      border: 2px solid rgba(0,0,0,0.2);
      border-top-color: var(--color-black);
      border-radius: 50%;
      animation: spin 0.6s linear infinite;
    }

    @media (max-width: 1024px) {
      .checkout-layout { grid-template-columns: 1fr; }
      .order-summary { position: static; }
    }

    @media (max-width: 640px) {
      .form-grid { grid-template-columns: 1fr; }
    }
  `]
})
export class CheckoutComponent implements OnInit {
  private cartService = inject(CartService);
  private orderService = inject(OrderService);
  private toast = inject(ToastService);
  private router = inject(Router);
  private paymentService = inject(PaymentService);

  cart: CartResponse | null = null;
  paymentMethod = 'cod';
  isSubmitting = false;
  errorMessage = '';

  ngOnInit() {
    this.cartService.cart$.subscribe((cart) => { this.cart = cart; });
    this.cartService.loadCart().subscribe();
  }

  placeOrder() {
    this.errorMessage = '';
    this.isSubmitting = true;

    const orderData = {
      paymentMethod: this.paymentMethod === 'cod' ? PaymentMethod.CashOnDelivery : PaymentMethod.Stripe,
      notes: ''
    };

    this.orderService.placeOrder(orderData).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          const orderId = response.data.orderId;
          this.cartService.loadCart().subscribe(); // Refresh cart (should be empty)

          if (this.paymentMethod === 'stripe') {
            this.paymentService.initiatePayment({ orderId: orderId, paymentMethod: PaymentMethod.Stripe, currency: 'EGP' })
              .pipe(finalize(() => this.isSubmitting = false))
              .subscribe({
                next: (payRes) => {
                  if (payRes.success) {
                    this.toast.success('Order placed & payment initiated! (Check console)');
                    console.log('Stripe intent:', payRes.data);
                  } else {
                    this.toast.error(payRes.message || 'Failed to initiate payment.');
                  }
                  this.router.navigate(['/profile/orders', orderId]);
                },
                error: (error) => {
                  this.toast.error('Error initiating payment.');
                  console.error('Payment initiation error:', error);
                  this.router.navigate(['/profile/orders', orderId]);
                }
              });
          } else {
            this.isSubmitting = false;
            this.toast.success('Order placed successfully! 🎉');
            this.router.navigate(['/profile/orders', orderId]);
          }
        } else {
          this.isSubmitting = false;
          this.errorMessage = response.message || 'Failed to place order. Please try again.';
        }
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = 'An error occurred while placing your order. Please try again.';
        console.error('Order placement error:', error);
      }
    });
  }
}
