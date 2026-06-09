import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="auth-page">
      <div class="auth-card card card-featured">
        <div class="auth-header">
          <h1 class="heading-4">Reset your password</h1>
          <p class="text-muted body-small">Enter your new password below</p>
        </div>

        <form (ngSubmit)="onSubmit()" class="auth-form">
          <div class="form-group">
            <label for="newPassword" class="form-label">New Password</label>
            <input type="password" id="newPassword" class="input" [(ngModel)]="newPassword" name="newPassword" placeholder="Min 8 characters" required/>
          </div>

          <div class="form-group">
            <label for="confirmPassword" class="form-label">Confirm Password</label>
            <input type="password" id="confirmPassword" class="input" [(ngModel)]="confirmPassword" name="confirmPassword" placeholder="Re-enter password" required/>
          </div>

          @if (errorMessage) { <p class="form-error">{{ errorMessage }}</p> }

          <button type="submit" class="btn btn-primary w-full btn-lg" [disabled]="isLoading" id="reset-submit">
            {{ isLoading ? 'Resetting...' : 'Reset password' }}
          </button>
        </form>

        <p class="auth-footer text-muted body-small">
          <a routerLink="/login" class="text-white">← Back to sign in</a>
        </p>
      </div>
    </div>
  `,
  styles: [`
    .auth-page { min-height: 100vh; display: flex; align-items: center; justify-content: center; padding: var(--space-4); background: radial-gradient(ellipse at center, var(--color-dark-forest) 0%, var(--color-void) 70%); }
    .auth-card { width: 100%; max-width: 440px; padding: var(--space-9); }
    .auth-header { text-align: center; display: flex; flex-direction: column; align-items: center; gap: var(--space-3); margin-bottom: var(--space-7); }
    .auth-form { display: flex; flex-direction: column; gap: var(--space-5); }
    .auth-footer { text-align: center; margin-top: var(--space-5); }
    .auth-footer a { font-weight: 500; }
  `]
})
export class ResetPasswordComponent implements OnInit {
  private authService = inject(AuthService);
  private toast = inject(ToastService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  email = '';
  token = '';
  newPassword = '';
  confirmPassword = '';
  isLoading = false;
  errorMessage = '';

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      this.email = params['email'] || '';
      this.token = params['token'] || '';
      if (!this.email || !this.token) {
        this.errorMessage = 'Invalid or incomplete reset link.';
      }
    });
  }

  onSubmit() {
    if (!this.email || !this.token) { this.errorMessage = 'Invalid or incomplete reset link.'; return; }
    if (!this.newPassword || !this.confirmPassword) { this.errorMessage = 'Please fill in all fields.'; return; }
    if (this.newPassword !== this.confirmPassword) { this.errorMessage = 'Passwords do not match.'; return; }

    this.isLoading = true;
    this.authService.resetPassword({ email: this.email, token: this.token, newPassword: this.newPassword }).subscribe({
      next: (res) => {
        this.isLoading = false;
        if (res.success) { this.toast.success('Password reset successfully!'); this.router.navigate(['/login']); }
        else { this.errorMessage = res.message || 'Reset failed.'; }
      },
      error: (err) => { this.isLoading = false; this.errorMessage = err.error?.message || 'Reset failed.'; },
    });
  }
}
