import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-register',
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
          <h1 class="heading-4">Create your account</h1>
          <p class="text-muted body-small">Start your shopping journey today</p>
        </div>

        <form (ngSubmit)="onSubmit()" class="auth-form">
          <div class="form-group">
            <label for="fullName" class="form-label">Full Name</label>
            <input type="text" id="fullName" class="input" [(ngModel)]="fullName" name="fullName" placeholder="John Doe" required autocomplete="name"/>
          </div>

          <div class="form-group">
            <label for="email" class="form-label">Email</label>
            <input type="email" id="email" class="input" [(ngModel)]="email" name="email" placeholder="you@example.com" required autocomplete="email"/>
          </div>

          <div class="form-group">
            <label for="phone" class="form-label">Phone Number</label>
            <input type="tel" id="phone" class="input" [(ngModel)]="phone" name="phone" placeholder="01xxxxxxxxx" required autocomplete="tel"/>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="password" class="form-label">Password</label>
              <input type="password" id="password" class="input" [(ngModel)]="password" name="password" placeholder="Min 8 characters" required autocomplete="new-password"/>
            </div>
            <div class="form-group">
              <label for="confirmPassword" class="form-label">Confirm Password</label>
              <input type="password" id="confirmPassword" class="input" [(ngModel)]="confirmPassword" name="confirmPassword" placeholder="Re-enter password" required autocomplete="new-password"/>
            </div>
          </div>

          @if (errorMessage) {
            <p class="form-error">{{ errorMessage }}</p>
          }

          @if (errors.length > 0) {
            <div class="form-errors">
              @for (error of errors; track error) {
                <p class="form-error">• {{ error }}</p>
              }
            </div>
          }

          <button type="submit" class="btn btn-primary w-full btn-lg" [disabled]="isLoading" id="register-submit">
            @if (isLoading) {
              <span class="btn-spinner"></span>
            }
            {{ isLoading ? 'Creating account...' : 'Create account' }}
          </button>
        </form>

        <p class="auth-footer text-muted body-small">
          Already have an account?
          <a routerLink="/login" class="text-white" id="login-link">Sign in</a>
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
      max-width: 520px;
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

    .auth-logo { margin-bottom: var(--space-2); }

    .auth-form {
      display: flex;
      flex-direction: column;
      gap: var(--space-5);
    }

    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-4);
    }

    .form-errors {
      display: flex;
      flex-direction: column;
      gap: var(--space-1);
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
      .auth-card { padding: var(--space-5); }
      .form-row { grid-template-columns: 1fr; }
    }
  `]
})
export class RegisterComponent {
  private authService = inject(AuthService);
  private toast = inject(ToastService);
  private router = inject(Router);

  private readonly emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  private readonly phoneRegex = /^01[0-2,5][0-9]{8}$/;

  fullName = '';
  email = '';
  phone = '';
  password = '';
  confirmPassword = '';
  isLoading = false;
  errorMessage = '';
  errors: string[] = [];

  onSubmit() {
    this.errorMessage = '';
    this.errors = [];

    if (!this.validateClientSide()) {
      return;
    }

    this.isLoading = true;
    this.authService
      .register({
        fullName: this.fullName,
        email: this.email,
        phone: this.phone,
        password: this.password,
        confirmPassword: this.confirmPassword,
      })
      .subscribe({
        next: (res) => {
          this.isLoading = false;
          if (res.success) {
            this.toast.success('Account created! Please check your email to verify.');
            this.router.navigate(['/login']);
          } else {
            this.errorMessage = res.message || 'Registration failed.';
            this.errors = res.errors ?? [];
          }
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage = err.error?.message || 'Registration failed.';
          this.errors = err.error?.errors ?? [];
        },
      });
  }

  private validateClientSide(): boolean {
    const validationErrors: string[] = [];

    if (!this.fullName.trim()) {
      validationErrors.push('Full name is required.');
    } else if (this.fullName.trim().length < 3) {
      validationErrors.push('Full name must be at least 3 characters.');
    }

    if (!this.email.trim()) {
      validationErrors.push('Email is required.');
    } else if (!this.emailRegex.test(this.email.trim())) {
      validationErrors.push('Please enter a valid email address.');
    }

    if (!this.phone.trim()) {
      validationErrors.push('Phone number is required.');
    } else if (!this.phoneRegex.test(this.phone.trim())) {
      validationErrors.push('Phone number must be a valid Egyptian mobile number (01xxxxxxxxx).');
    }

    if (!this.password) {
      validationErrors.push('Password is required.');
    } else {
      if (this.password.length < 8) validationErrors.push('Password must be at least 8 characters.');
      if (!/[A-Z]/.test(this.password)) validationErrors.push('Password must contain at least one uppercase letter.');
      if (!/[a-z]/.test(this.password)) validationErrors.push('Password must contain at least one lowercase letter.');
      if (!/[0-9]/.test(this.password)) validationErrors.push('Password must contain at least one number.');
      if (!/[^A-Za-z0-9]/.test(this.password)) validationErrors.push('Password must contain at least one special character.');
    }

    if (!this.confirmPassword) {
      validationErrors.push('Please confirm your password.');
    } else if (this.password !== this.confirmPassword) {
      validationErrors.push('Passwords do not match.');
    }

    if (validationErrors.length > 0) {
      this.errorMessage = 'Please fix the validation errors below.';
      this.errors = validationErrors;
      return false;
    }

    return true;
  }
}
