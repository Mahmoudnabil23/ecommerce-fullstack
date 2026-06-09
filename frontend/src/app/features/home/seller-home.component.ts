import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-seller-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <section class="seller-hero">
      <div class="container seller-hero-content">
        <span class="overline text-neon">Seller Workspace</span>
        <h1 class="display-light">Manage your store from one place</h1>
        <p class="body-large text-muted seller-subtitle">
          Keep your catalog updated, follow incoming orders, and monitor your business performance.
        </p>
        <div class="seller-actions">
          <a routerLink="/products" class="btn btn-primary btn-lg">View marketplace</a>
          <a routerLink="/profile" class="btn btn-ghost btn-lg">Store profile</a>
        </div>
      </div>
    </section>

    <section class="section">
      <div class="container seller-grid">
        <article class="seller-card">
          <h3 class="heading-4">Product operations</h3>
          <p class="text-muted">Add and maintain product listings, media, pricing, and stock levels.</p>
        </article>

        <article class="seller-card">
          <h3 class="heading-4">Order pipeline</h3>
          <p class="text-muted">Review order updates and keep fulfillment status accurate.</p>
        </article>

        <article class="seller-card">
          <h3 class="heading-4">Performance insights</h3>
          <p class="text-muted">Track your growth with revenue, top products, and conversion trends.</p>
        </article>
      </div>
    </section>
  `,
  styles: [`
    .seller-hero {
      min-height: 65vh;
      display: flex;
      align-items: center;
      background: radial-gradient(circle at 80% 10%, #0f4f41, var(--color-void) 72%);
    }

    .seller-hero-content {
      display: flex;
      flex-direction: column;
      gap: var(--space-5);
      padding: var(--space-10) 0;
    }

    .seller-subtitle {
      max-width: 640px;
    }

    .seller-actions {
      display: flex;
      flex-wrap: wrap;
      gap: var(--space-3);
    }

    .seller-grid {
      display: grid;
      grid-template-columns: repeat(3, minmax(0, 1fr));
      gap: var(--space-5);
    }

    .seller-card {
      border: 1px solid var(--color-dark-card-border);
      border-radius: var(--radius-2xl);
      padding: var(--space-6);
      background: rgba(255, 255, 255, 0.02);
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
    }

    @media (max-width: 900px) {
      .seller-grid {
        grid-template-columns: 1fr;
      }
    }
  `],
})
export class SellerHomeComponent {}
