import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="auth-page">
      <div class="auth-card card card-featured">
        <div class="auth-header">
          <h1 class="heading-4">Forgot password?</h1>
          <p class="text-muted body-small">Enter your email and we'll send you a reset link</p>
        </div>

        @if (!sent) {
          <form (ngSubmit)="onSubmit()" class="auth-form">
            <div class="form-group">
              <label for="email" class="form-label">Email</label>
              <input type="email" id="email" class="input" [(ngModel)]="email" name="email" placeholder="you@example.com" required/>
            </div>

            @if (errorMessage) {
              <p class="form-error">{{ errorMessage }}</p>
            }

            <button type="submit" class="btn btn-primary w-full btn-lg" [disabled]="isLoading" id="forgot-submit">
              {{ isLoading ? 'Sending...' : 'Send reset link' }}
            </button>
          </form>
        } @else {
          <div class="success-msg">
            <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="#36F4A4" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>
            <p class="body-large text-white">Check your email</p>
            <p class="text-muted body-small">We've sent a password reset link to your email address.</p>
          </div>
        }

        <p class="auth-footer text-muted body-small">
          <a routerLink="/login" class="text-white">← Back to sign in</a>
        </p>
      </div>
    </div>
  `,
  styles: [`
    .auth-page {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: var(--space-4);
      background: radial-gradient(ellipse at center, var(--color-dark-forest) 0%, var(--color-void) 70%);
    }
    .auth-card { width: 100%; max-width: 440px; padding: var(--space-9); }
    .auth-header { text-align: center; display: flex; flex-direction: column; align-items: center; gap: var(--space-3); margin-bottom: var(--space-7); }
    .auth-form { display: flex; flex-direction: column; gap: var(--space-5); }
    .auth-footer { text-align: center; margin-top: var(--space-5); }
    .auth-footer a { font-weight: 500; }
    .success-msg { text-align: center; display: flex; flex-direction: column; align-items: center; gap: var(--space-4); padding: var(--space-5) 0; }
  `]
})
export class ForgotPasswordComponent {
  private authService = inject(AuthService);
  private toast = inject(ToastService);

  email = '';
  isLoading = false;
  errorMessage = '';
  sent = false;

  onSubmit() {
    if (!this.email) { this.errorMessage = 'Please enter your email.'; return; }
    this.isLoading = true;
    this.authService.forgotPassword({ email: this.email }).subscribe({
      next: () => { this.isLoading = false; this.sent = true; },
      error: (err) => { this.isLoading = false; this.errorMessage = err.error?.message || 'Failed to send reset link.'; },
    });
  }
}
