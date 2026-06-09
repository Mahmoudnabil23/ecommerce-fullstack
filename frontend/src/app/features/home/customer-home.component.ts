import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-customer-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <section class="customer-hero">
      <div class="container customer-hero-content">
        <span class="overline text-neon">Welcome Back</span>
        <h1 class="display-light">Your shopping dashboard</h1>
        <p class="body-large text-muted customer-subtitle">
          Continue browsing products, track your latest orders, and finish checkout in a few clicks.
        </p>
        <div class="customer-actions">
          <a routerLink="/products" class="btn btn-primary btn-lg">Browse products</a>
          <a routerLink="/profile/orders" class="btn btn-ghost btn-lg">View orders</a>
          <a routerLink="/cart" class="btn btn-ghost btn-lg">Go to cart</a>
        </div>
      </div>
    </section>

    <section class="section surface-dark-forest">
      <div class="container quick-grid">
        <a class="quick-card" routerLink="/products">
          <h3 class="heading-4">Discover products</h3>
          <p class="text-muted">Search by category, price, and top rating.</p>
        </a>

        <a class="quick-card" routerLink="/profile/orders">
          <h3 class="heading-4">Track orders</h3>
          <p class="text-muted">See status updates from placed to delivered.</p>
        </a>

        <a class="quick-card" routerLink="/profile">
          <h3 class="heading-4">Manage profile</h3>
          <p class="text-muted">Update account details and addresses anytime.</p>
        </a>
      </div>
    </section>
  `,
  styles: [`
    .customer-hero {
      min-height: 65vh;
      display: flex;
      align-items: center;
      background: radial-gradient(circle at 20% 20%, var(--color-forest), var(--color-void) 70%);
    }

    .customer-hero-content {
      display: flex;
      flex-direction: column;
      align-items: flex-start;
      gap: var(--space-5);
      padding: var(--space-10) 0;
    }

    .customer-subtitle {
      max-width: 640px;
    }

    .customer-actions {
      display: flex;
      flex-wrap: wrap;
      gap: var(--space-3);
    }

    .quick-grid {
      display: grid;
      grid-template-columns: repeat(3, minmax(0, 1fr));
      gap: var(--space-5);
    }

    .quick-card {
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
      padding: var(--space-6);
      border: 1px solid var(--color-dark-card-border);
      border-radius: var(--radius-2xl);
      background: rgba(255, 255, 255, 0.03);
      text-decoration: none;
      color: var(--color-white);
      transition: transform var(--transition-fast), border-color var(--transition-fast);
    }

    .quick-card:hover {
      transform: translateY(-4px);
      border-color: var(--color-neon-green);
    }

    @media (max-width: 900px) {
      .quick-grid {
        grid-template-columns: 1fr;
      }
    }
  `],
})
export class CustomerHomeComponent {}
