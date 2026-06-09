import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="auth-page">
      <div class="auth-card card card-featured">
        <h1 class="heading-4">{{ title }}</h1>
        <p class="text-muted body-small">{{ message }}</p>
        <a routerLink="/login" class="btn btn-primary w-full btn-lg">Go to sign in</a>
      </div>
    </div>
  `,
  styles: [`
    .auth-page { min-height: 100vh; display: flex; align-items: center; justify-content: center; padding: var(--space-4); background: radial-gradient(ellipse at center, var(--color-dark-forest) 0%, var(--color-void) 70%); }
    .auth-card { width: 100%; max-width: 440px; padding: var(--space-8); display: flex; flex-direction: column; gap: var(--space-4); text-align: center; }
  `]
})
export class VerifyEmailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private authService = inject(AuthService);
  private toast = inject(ToastService);
  private router = inject(Router);

  title = 'Verifying email...';
  message = 'Please wait.';

  ngOnInit(): void {
    const userId = this.route.snapshot.queryParamMap.get('userId') ?? '';
    const token = this.route.snapshot.queryParamMap.get('token') ?? '';
    if (!userId || !token) {
      this.title = 'Invalid verification link';
      this.message = 'Missing verification data.';
      return;
    }

    this.authService.verifyEmail(userId, token).subscribe({
      next: (res) => {
        if (res.success) {
          this.title = 'Email verified';
          this.message = res.message || 'Your email has been verified successfully.';
          this.toast.success('Email verified successfully.');
          this.router.navigate(['/login']);
          return;
        }
        this.title = 'Verification failed';
        this.message = res.message || 'Unable to verify email.';
      },
      error: (err) => {
        this.title = 'Verification failed';
        this.message = err.error?.message || 'Unable to verify email.';
      },
    });
  }
}
