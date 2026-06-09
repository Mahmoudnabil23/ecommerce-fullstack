import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="section">
      <div class="container">
        <h1 class="heading-2" style="margin-bottom: var(--space-9);">My Account</h1>

        <div class="profile-layout">
          <!-- User Info Card -->
          <div class="profile-card card">
            <div class="profile-avatar">
              <span class="avatar-initials heading-3">{{ getInitials() }}</span>
            </div>
            <div class="profile-info">
              <h2 class="heading-4">{{ authService.currentUser?.fullName ?? 'User' }}</h2>
              <p class="text-muted body-small">Member since joining</p>
            </div>
          </div>

          <!-- Quick Links -->
          <div class="profile-grid">
            <a routerLink="/profile/orders" class="profile-link-card card" id="profile-orders">
              <div class="profile-link-icon">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M6 2L3 6v14a2 2 0 002 2h14a2 2 0 002-2V6l-3-4z"/><line x1="3" y1="6" x2="21" y2="6"/></svg>
              </div>
              <div>
                <h3 class="body-medium">My Orders</h3>
                <p class="caption text-muted">Track and manage your orders</p>
              </div>
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="var(--color-shade-50)" stroke-width="2" class="profile-link-arrow"><path d="M9 18l6-6-6-6"/></svg>
            </a>

            <a routerLink="/cart" class="profile-link-card card" id="profile-cart">
              <div class="profile-link-icon">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="9" cy="21" r="1"/><circle cx="20" cy="21" r="1"/><path d="M1 1h4l2.68 13.39a2 2 0 002 1.61h9.72a2 2 0 002-1.61L23 6H6"/></svg>
              </div>
              <div>
                <h3 class="body-medium">Shopping Cart</h3>
                <p class="caption text-muted">View items in your cart</p>
              </div>
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="var(--color-shade-50)" stroke-width="2" class="profile-link-arrow"><path d="M9 18l6-6-6-6"/></svg>
            </a>

            <div class="profile-link-card card" id="profile-settings" style="cursor: default;">
              <div class="profile-link-icon">
                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="3"/><path d="M19.4 15a1.65 1.65 0 00.33 1.82l.06.06a2 2 0 010 2.83 2 2 0 01-2.83 0l-.06-.06a1.65 1.65 0 00-1.82-.33 1.65 1.65 0 00-1 1.51V21a2 2 0 01-2 2 2 2 0 01-2-2v-.09A1.65 1.65 0 009 19.4a1.65 1.65 0 00-1.82.33l-.06.06a2 2 0 01-2.83 0 2 2 0 010-2.83l.06-.06A1.65 1.65 0 004.68 15a1.65 1.65 0 00-1.51-1H3a2 2 0 01-2-2 2 2 0 012-2h.09A1.65 1.65 0 004.6 9a1.65 1.65 0 00-.33-1.82l-.06-.06a2 2 0 010-2.83 2 2 0 012.83 0l.06.06A1.65 1.65 0 009 4.68a1.65 1.65 0 001-1.51V3a2 2 0 012-2 2 2 0 012 2v.09a1.65 1.65 0 001 1.51 1.65 1.65 0 001.82-.33l.06-.06a2 2 0 012.83 0 2 2 0 010 2.83l-.06.06a1.65 1.65 0 00-.33 1.82V9a1.65 1.65 0 001.51 1H21a2 2 0 012 2 2 2 0 01-2 2h-.09a1.65 1.65 0 00-1.51 1z"/></svg>
              </div>
              <div>
                <h3 class="body-medium">Account Settings</h3>
                <p class="caption text-muted">Manage your account details</p>
              </div>
              <span class="coming-soon-badge caption">Coming soon</span>
            </div>
          </div>

          <!-- Logout -->
          <div class="card" style="margin-top: var(--space-5); padding: var(--space-5);">
            <h3 class="heading-6" style="margin-bottom: var(--space-3);">Change Password</h3>
            <form (ngSubmit)="changePassword()" class="change-password-form">
              <input type="password" class="input" [(ngModel)]="currentPassword" name="currentPassword" placeholder="Current password" required />
              <input type="password" class="input" [(ngModel)]="newPassword" name="newPassword" placeholder="New password" required />
              <button class="btn btn-primary" [disabled]="isSubmitting">{{ isSubmitting ? 'Updating...' : 'Update password' }}</button>
            </form>
          </div>

          <button class="btn btn-ghost w-full" (click)="authService.logout()" id="profile-logout" style="margin-top: var(--space-5);">
            Sign Out
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .profile-layout {
      max-width: 640px;
    }

    .profile-card {
      display: flex;
      align-items: center;
      gap: var(--space-5);
      padding: var(--space-7);
      margin-bottom: var(--space-7);
    }

    .profile-avatar {
      width: 72px;
      height: 72px;
      border-radius: 50%;
      background: linear-gradient(135deg, var(--color-forest), var(--color-neon-green));
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }

    .avatar-initials {
      color: var(--color-black);
      font-weight: 600;
      font-size: 24px;
    }

    .profile-info {
      display: flex;
      flex-direction: column;
      gap: var(--space-1);
    }

    .profile-grid {
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
    }

    .profile-link-card {
      display: flex;
      align-items: center;
      gap: var(--space-4);
      padding: var(--space-5);
      text-decoration: none;
      color: var(--color-white);
      cursor: pointer;
    }

    .profile-link-card:hover {
      background: var(--color-dark-forest);
    }

    .profile-link-icon {
      width: 44px;
      height: 44px;
      border-radius: var(--radius-sm);
      background: var(--color-dark-forest);
      display: flex;
      align-items: center;
      justify-content: center;
      color: var(--color-shade-30);
      flex-shrink: 0;
    }

    .profile-link-card:hover .profile-link-icon {
      color: var(--color-neon-green);
    }

    .profile-link-card div:nth-child(2) {
      flex: 1;
      display: flex;
      flex-direction: column;
      gap: 2px;
    }

    .profile-link-arrow {
      flex-shrink: 0;
    }

    .coming-soon-badge {
      background: var(--color-dark-forest);
      color: var(--color-muted);
      padding: 4px 10px;
      border-radius: var(--radius-pill);
      flex-shrink: 0;
    }

    .change-password-form {
      display: grid;
      gap: var(--space-3);
    }
  `]
})
export class ProfileComponent {
  authService = inject(AuthService);
  private toast = inject(ToastService);
  currentPassword = '';
  newPassword = '';
  isSubmitting = false;

  getInitials(): string {
    const name = this.authService.currentUser?.fullName ?? '';
    return name.split(' ').map(n => n[0] ?? '').join('').toUpperCase().slice(0, 2) || 'U';
  }

  changePassword(): void {
    if (!this.currentPassword || !this.newPassword) return;
    this.isSubmitting = true;
    this.authService.changePassword({ currentPassword: this.currentPassword, newPassword: this.newPassword }).subscribe({
      next: (res) => {
        this.isSubmitting = false;
        if (!res.success) {
          this.toast.error(res.message || 'Failed to change password');
          return;
        }
        this.currentPassword = '';
        this.newPassword = '';
        this.toast.success('Password changed successfully');
      },
      error: (err) => {
        this.isSubmitting = false;
        this.toast.error(err.error?.message || 'Failed to change password');
      }
    });
  }
}
