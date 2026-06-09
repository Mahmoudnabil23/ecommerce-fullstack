import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [RouterModule],
  template: `
    <footer class="footer">
      <div class="container">
        <div class="footer-grid">
          <div class="footer-brand">
            <a routerLink="/" class="footer-logo">
              <svg width="28" height="28" viewBox="0 0 32 32" fill="none">
                <rect width="32" height="32" rx="8" fill="#36F4A4"/>
                <path d="M8 16L14 22L24 10" stroke="#000" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
              <span>AssiutCart</span>
            </a>
            <p class="footer-tagline text-muted">Premium shopping experience, delivered to your door.</p>
          </div>

          <div class="footer-col">
            <h4 class="footer-heading">Shop</h4>
            <a routerLink="/products" class="footer-link">All Products</a>
            <a routerLink="/products" [queryParams]="{sortBy: 'newest'}" class="footer-link">New Arrivals</a>
            <a routerLink="/products" [queryParams]="{sortBy: 'rating'}" class="footer-link">Top Rated</a>
          </div>

          <div class="footer-col">
            <h4 class="footer-heading">Account</h4>
            <a routerLink="/login" class="footer-link">Sign In</a>
            <a routerLink="/register" class="footer-link">Create Account</a>
            <a routerLink="/cart" class="footer-link">Shopping Cart</a>
          </div>

          <div class="footer-col">
            <h4 class="footer-heading">Help</h4>
            <a class="footer-link">Shipping Info</a>
            <a class="footer-link">Returns</a>
            <a class="footer-link">Contact Us</a>
          </div>
        </div>

        <div class="footer-bottom">
          <p class="text-shade-50 caption">© 2026 AssiutCart. All rights reserved.</p>
        </div>
      </div>
    </footer>
  `,
  styles: [`
    .footer {
      background: var(--color-deep-teal);
      border-top: 1px solid var(--color-dark-card-border);
      padding: 64px 0 32px;
    }

    .footer-grid {
      display: grid;
      grid-template-columns: 2fr 1fr 1fr 1fr;
      gap: var(--space-10);
    }

    .footer-brand {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
    }

    .footer-logo {
      display: flex;
      align-items: center;
      gap: var(--space-3);
      color: var(--color-white);
      font-family: var(--font-display);
      font-size: 18px;
      font-weight: 600;
    }

    .footer-tagline {
      font-size: 15px;
      line-height: 1.5;
      max-width: 280px;
    }

    .footer-col {
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
    }

    .footer-heading {
      font-family: var(--font-display);
      font-size: 14px;
      font-weight: 500;
      color: var(--color-shade-30);
      letter-spacing: 0.72px;
      text-transform: uppercase;
      margin-bottom: var(--space-1);
    }

    .footer-link {
      font-size: 15px;
      color: var(--color-muted);
      transition: color var(--transition-fast);
      cursor: pointer;
    }

    .footer-link:hover {
      color: var(--color-white);
    }

    .footer-bottom {
      margin-top: 48px;
      padding-top: var(--space-5);
      border-top: 1px solid var(--color-shade-70);
      text-align: center;
    }

    @media (max-width: 768px) {
      .footer-grid {
        grid-template-columns: 1fr 1fr;
        gap: var(--space-7);
      }

      .footer-brand {
        grid-column: 1 / -1;
      }
    }

    @media (max-width: 480px) {
      .footer-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class FooterComponent {}
