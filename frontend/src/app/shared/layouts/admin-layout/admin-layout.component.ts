import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterModule],
  template: `
    <div class="admin-layout">
      <aside class="admin-sidebar">
        <div class="sidebar-header">
          <a routerLink="/" class="sidebar-logo">
            <svg width="28" height="28" viewBox="0 0 32 32" fill="none">
              <rect width="32" height="32" rx="8" fill="#36F4A4"/>
              <path d="M8 16L14 22L24 10" stroke="#000" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            <span>Admin Panel</span>
          </a>
        </div>

        <nav class="sidebar-nav">
          <a routerLink="/admin/dashboard" routerLinkActive="sidebar-active" class="sidebar-link" id="admin-nav-dashboard">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/><rect x="14" y="14" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/></svg>
            Dashboard
          </a>
          <a routerLink="/admin/products" routerLinkActive="sidebar-active" class="sidebar-link" id="admin-nav-products">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M6 2L3 6v14a2 2 0 002 2h14a2 2 0 002-2V6l-3-4z"/><line x1="3" y1="6" x2="21" y2="6"/><path d="M16 10a4 4 0 01-8 0"/></svg>
            Products
          </a>
          <a routerLink="/admin/categories" routerLinkActive="sidebar-active" class="sidebar-link" id="admin-nav-categories">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/><rect x="14" y="14" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/></svg>
            Categories
          </a>
          <a routerLink="/admin/users" routerLinkActive="sidebar-active" class="sidebar-link" id="admin-nav-users">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 00-3-3.87"/><path d="M16 3.13a4 4 0 010 7.75"/></svg>
            Users
          </a>
        </nav>

        <div class="sidebar-footer">
          <a routerLink="/" class="sidebar-link">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M15 18l-6-6 6-6"/></svg>
            Back to Store
          </a>
          <button class="sidebar-link" (click)="authService.logout()" id="admin-logout">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M9 21H5a2 2 0 01-2-2V5a2 2 0 012-2h4"/><polyline points="16 17 21 12 16 7"/><line x1="21" y1="12" x2="9" y2="12"/></svg>
            Logout
          </button>
        </div>
      </aside>

      <main class="admin-main">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [`
    .admin-layout {
      display: flex;
      min-height: 100vh;
    }

    .admin-sidebar {
      width: 260px;
      background: var(--color-deep-teal);
      border-right: 1px solid var(--color-dark-card-border);
      display: flex;
      flex-direction: column;
      padding: var(--space-5);
      position: fixed;
      top: 0;
      bottom: 0;
      left: 0;
      z-index: 100;
    }

    .sidebar-header {
      padding-bottom: var(--space-5);
      border-bottom: 1px solid var(--color-shade-70);
      margin-bottom: var(--space-5);
    }

    .sidebar-logo {
      display: flex;
      align-items: center;
      gap: var(--space-3);
      color: var(--color-white);
      font-family: var(--font-display);
      font-size: 18px;
      font-weight: 600;
      text-decoration: none;
    }

    .sidebar-nav {
      display: flex;
      flex-direction: column;
      gap: var(--space-1);
      flex: 1;
    }

    .sidebar-link {
      display: flex;
      align-items: center;
      gap: var(--space-3);
      padding: var(--space-3) var(--space-4);
      border-radius: var(--radius-sm);
      color: var(--color-muted);
      font-size: 15px;
      font-weight: 500;
      transition: all var(--transition-fast);
      text-decoration: none;
      width: 100%;
      text-align: left;
    }

    .sidebar-link:hover {
      color: var(--color-white);
      background: var(--color-dark-forest);
    }

    .sidebar-active {
      color: var(--color-white) !important;
      background: var(--color-forest) !important;
    }

    .sidebar-footer {
      display: flex;
      flex-direction: column;
      gap: var(--space-1);
      padding-top: var(--space-5);
      border-top: 1px solid var(--color-shade-70);
    }

    .admin-main {
      flex: 1;
      margin-left: 260px;
      padding: var(--space-7);
      min-height: 100vh;
      background: var(--color-void);
    }

    @media (max-width: 1024px) {
      .admin-sidebar {
        width: 72px;
        padding: var(--space-3);
        align-items: center;
      }

      .sidebar-logo span,
      .sidebar-link span,
      .sidebar-link:not(.sidebar-active)::after {
        display: none;
      }

      .sidebar-link {
        justify-content: center;
        padding: var(--space-3);
        font-size: 0;
      }

      .admin-main {
        margin-left: 72px;
      }
    }
  `]
})
export class AdminLayoutComponent {
  authService = inject(AuthService);
}
