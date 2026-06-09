import { Component, inject, HostListener, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CartService } from '../../../core/services/cart.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav class="navbar" [class.scrolled]="isScrolled()">
      <div class="nav-container container">
        <!-- Logo -->
        <a [routerLink]="getHomeRoute()" class="nav-logo" id="nav-logo">
          <svg width="32" height="32" viewBox="0 0 32 32" fill="none">
            <rect width="32" height="32" rx="8" fill="#36F4A4"/>
            <path d="M8 16L14 22L24 10" stroke="#000" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          <span class="logo-text">AssiutCart</span>
        </a>

        <!-- Desktop Nav Links -->
        <div class="nav-links" [class.nav-open]="menuOpen()">
          <a [routerLink]="getHomeRoute()" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" class="nav-link" id="nav-home" (click)="closeMenu()">Home</a>
          <a routerLink="/products" routerLinkActive="active" class="nav-link" id="nav-products" (click)="closeMenu()">Products</a>

          @if (authService.isLoggedIn) {
            <a routerLink="/cart" routerLinkActive="active" class="nav-link nav-link-mobile" id="nav-cart-mobile" (click)="closeMenu()">Cart</a>
            <a routerLink="/profile" routerLinkActive="active" class="nav-link nav-link-mobile" id="nav-profile-mobile" (click)="closeMenu()">Profile</a>
            @if (authService.isAdmin) {
              <a routerLink="/admin/dashboard" routerLinkActive="active" class="nav-link nav-link-mobile" id="nav-admin-mobile" (click)="closeMenu()">Admin</a>
            }
            <button class="btn btn-ghost btn-sm nav-link-mobile" id="nav-logout-mobile" (click)="logout()">Logout</button>
          } @else {
            <a routerLink="/login" class="nav-link nav-link-mobile" id="nav-login-mobile" (click)="closeMenu()">Log in</a>
            <a routerLink="/register" class="btn btn-primary btn-sm nav-link-mobile" id="nav-register-mobile" (click)="closeMenu()">Start for free</a>
          }
        </div>

        <!-- Right Actions -->
        <div class="nav-actions">
          @if (authService.isLoggedIn) {
            <a routerLink="/cart" class="nav-cart-btn" id="nav-cart">
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <circle cx="9" cy="21" r="1"/><circle cx="20" cy="21" r="1"/>
                <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"/>
              </svg>
              @if ((cartService.cartItemCount$ | async) ?? 0; as count) {
                @if (count > 0) {
                  <span class="cart-badge">{{ count }}</span>
                }
              }
            </a>

            @if (authService.isAdmin) {
              <a routerLink="/admin/dashboard" class="btn btn-ghost btn-sm nav-desktop-only" id="nav-admin">Admin</a>
            }

            <a routerLink="/profile" class="btn btn-ghost btn-sm nav-desktop-only" id="nav-profile">
              {{ getDisplayName() }}
            </a>

            <button class="btn btn-primary btn-sm nav-desktop-only" id="nav-logout" (click)="logout()">Logout</button>
          } @else {
            <a routerLink="/login" class="btn btn-ghost btn-sm nav-desktop-only" id="nav-login">Log in</a>
            <a routerLink="/register" class="btn btn-primary btn-sm nav-desktop-only" id="nav-register">Start for free</a>
          }

          <!-- Hamburger -->
          <button class="hamburger" [class.open]="menuOpen()" (click)="toggleMenu()" id="nav-hamburger" aria-label="Toggle menu">
            <span></span><span></span><span></span>
          </button>
        </div>
      </div>
    </nav>
  `,
  styles: [`
    .navbar {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      z-index: 1000;
      height: 72px;
      background: transparent;
      transition: background var(--transition-normal);
    }

    .navbar.scrolled {
      background: var(--color-forest);
      backdrop-filter: blur(12px);
      border-bottom: 1px solid var(--color-dark-card-border);
    }

    .nav-container {
      display: flex;
      align-items: center;
      justify-content: space-between;
      height: 100%;
    }

    .nav-logo {
      display: flex;
      align-items: center;
      gap: var(--space-3);
      text-decoration: none;
      color: var(--color-white);
      z-index: 1001;
    }

    .logo-text {
      font-family: var(--font-display);
      font-size: 20px;
      font-weight: 600;
      letter-spacing: 0.5px;
    }

    .nav-links {
      display: flex;
      align-items: center;
      gap: var(--space-7);
    }

    .nav-link {
      font-family: var(--font-display);
      font-size: 16px;
      font-weight: 500;
      color: var(--color-white);
      letter-spacing: 0.72px;
      transition: color var(--transition-fast);
      text-decoration: none;
      padding: 8px 0;
    }

    .nav-link:hover, .nav-link.active {
      color: var(--color-neon-green);
    }

    .nav-link-mobile {
      display: none;
    }

    .nav-actions {
      display: flex;
      align-items: center;
      gap: var(--space-3);
    }

    .nav-cart-btn {
      position: relative;
      display: flex;
      align-items: center;
      color: var(--color-white);
      transition: color var(--transition-fast);
      padding: 8px;
    }

    .nav-cart-btn:hover {
      color: var(--color-neon-green);
    }

    .cart-badge {
      position: absolute;
      top: 0;
      right: 0;
      background: var(--color-neon-green);
      color: var(--color-black);
      font-size: 11px;
      font-weight: 600;
      width: 18px;
      height: 18px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .hamburger {
      display: none;
      flex-direction: column;
      gap: 5px;
      padding: 8px;
      z-index: 1001;
    }

    .hamburger span {
      width: 24px;
      height: 2px;
      background: var(--color-white);
      transition: all var(--transition-fast);
    }

    .hamburger.open span:nth-child(1) {
      transform: rotate(45deg) translate(5px, 5px);
    }
    .hamburger.open span:nth-child(2) {
      opacity: 0;
    }
    .hamburger.open span:nth-child(3) {
      transform: rotate(-45deg) translate(5px, -5px);
    }

    .nav-desktop-only { }

    @media (max-width: 1024px) {
      .nav-links {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        flex-direction: column;
        justify-content: center;
        background: var(--color-void);
        transform: translateX(100%);
        transition: transform var(--transition-normal);
        z-index: 1000;
        gap: var(--space-5);
      }

      .nav-links.nav-open {
        transform: translateX(0);
      }

      .nav-link {
        font-size: 24px;
      }

      .nav-link-mobile {
        display: block;
      }

      .nav-desktop-only {
        display: none;
      }

      .hamburger {
        display: flex;
      }
    }
  `]
})
export class NavbarComponent {
  authService = inject(AuthService);
  cartService = inject(CartService);

  isScrolled = signal(false);
  menuOpen = signal(false);

  @HostListener('window:scroll')
  onScroll() {
    this.isScrolled.set(window.scrollY > 50);
  }

  toggleMenu() {
    this.menuOpen.update((v) => !v);
  }

  closeMenu() {
    this.menuOpen.set(false);
  }

  logout() {
    this.closeMenu();
    this.authService.logout();
  }

  getDisplayName(): string {
    const name = this.authService.currentUser?.fullName;
    if (!name) return 'Profile';
    return name.split(' ')[0];
  }

  getHomeRoute(): string {
    return this.authService.getDefaultRoute();
  }
}
