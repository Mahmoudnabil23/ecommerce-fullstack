import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="auth-page">
      <div class="auth-card card card-featured">
        <div class="auth-header">
          <a routerLink="/" class="auth-logo">
            <svg width="36" height="36" viewBox="0 0 32 32" fill="none">
              <rect width="32" height="32" rx="8" fill="#36F4A4"/>
              <path d="M8 16L14 22L24 10" stroke="#000" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </a>
          <h1 class="heading-4">Welcome back</h1>
          <p class="text-muted body-small">Sign in to your account to continue</p>
        </div>

        <form (ngSubmit)="onSubmit()" class="auth-form">
          <div class="form-group">
            <label for="email" class="form-label">Email</label>
            <input
              type="email"
              id="email"
              class="input"
              [(ngModel)]="email"
              name="email"
              placeholder="you@example.com"
              required
              autocomplete="email"/>
          </div>

          <div class="form-group">
            <label for="password" class="form-label">Password</label>
            <input
              type="password"
              id="password"
              class="input"
              [(ngModel)]="password"
              name="password"
              placeholder="Enter your password"
              required
              autocomplete="current-password"/>
          </div>

          <div class="auth-forgot">
            <a routerLink="/forgot-password" class="text-muted caption" id="forgot-password-link">Forgot password?</a>
          </div>

          @if (errorMessage) {
            <p class="form-error">{{ errorMessage }}</p>
          }

          <button type="submit" class="btn btn-primary w-full btn-lg" [disabled]="isLoading" id="login-submit">
            @if (isLoading) {
              <span class="btn-spinner"></span>
            }
            {{ isLoading ? 'Signing in...' : 'Sign in' }}
          </button>
        </form>

        <p class="auth-footer text-muted body-small">
          Don't have an account?
          <a routerLink="/register" class="text-white" id="register-link">Create one</a>
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

    .auth-card {
      width: 100%;
      max-width: 440px;
      padding: var(--space-9);
    }

    .auth-header {
      text-align: center;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-3);
      margin-bottom: var(--space-7);
    }

    .auth-logo {
      margin-bottom: var(--space-2);
    }

    .auth-form {
      display: flex;
      flex-direction: column;
      gap: var(--space-5);
    }

    .auth-forgot {
      text-align: right;
      margin-top: calc(-1 * var(--space-3));
    }

    .auth-forgot a:hover {
      color: var(--color-white);
    }

    .auth-footer {
      text-align: center;
      margin-top: var(--space-5);
    }

    .auth-footer a {
      font-weight: 500;
      text-decoration: underline;
    }

    .btn-spinner {
      width: 18px;
      height: 18px;
      border: 2px solid rgba(0,0,0,0.2);
      border-top-color: var(--color-black);
      border-radius: 50%;
      animation: spin 0.6s linear infinite;
    }

    @media (max-width: 640px) {
      .auth-card {
        padding: var(--space-5);
      }
    }
  `]
})
export class LoginComponent {
  private authService = inject(AuthService);
  private toast = inject(ToastService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  email = '';
  password = '';
  isLoading = false;
  errorMessage = '';

  onSubmit() {
    this.errorMessage = '';
    if (!this.email || !this.password) {
      this.errorMessage = 'Please fill in all fields.';
      return;
    }

    this.isLoading = true;
    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.success) {
          this.toast.success('Welcome back!');
          const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
          const targetUrl = returnUrl || this.authService.getDefaultRoute();
          this.router.navigateByUrl(targetUrl);
        } else {
          this.errorMessage = res.message || 'Login failed.';
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Invalid email or password.';
      },
    });
  }
}
