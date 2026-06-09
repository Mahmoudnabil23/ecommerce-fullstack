import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService, Toast } from '../../../core/services/toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="toast-container">
      @for (toast of (toastService.toasts$ | async) ?? []; track toast.id) {
        <div class="toast" [class]="'toast-' + toast.type" [id]="'toast-' + toast.id">
          <div class="toast-icon">
            @switch (toast.type) {
              @case ('success') {
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M20 6L9 17l-5-5"/></svg>
              }
              @case ('error') {
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M15 9l-6 6M9 9l6 6"/></svg>
              }
              @case ('warning') {
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>
              }
              @default {
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M12 16v-4M12 8h.01"/></svg>
              }
            }
          </div>
          <span class="toast-message">{{ toast.message }}</span>
          <button class="toast-close" (click)="toastService.dismiss(toast.id)" aria-label="Dismiss">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M18 6L6 18M6 6l12 12"/></svg>
          </button>
        </div>
      }
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      top: 88px;
      right: 24px;
      z-index: 9999;
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
      max-width: 420px;
    }

    .toast {
      display: flex;
      align-items: center;
      gap: var(--space-3);
      padding: var(--space-4) var(--space-4);
      border-radius: var(--radius-sm);
      background: var(--color-forest);
      border: 1px solid var(--color-dark-card-border);
      box-shadow: var(--shadow-high);
      animation: slideIn 0.3s ease forwards;
      color: var(--color-white);
    }

    .toast-success { border-left: 3px solid var(--color-neon-green); }
    .toast-error { border-left: 3px solid #ef4444; }
    .toast-warning { border-left: 3px solid #FBBF24; }
    .toast-info { border-left: 3px solid #60A5FA; }

    .toast-success .toast-icon { color: var(--color-neon-green); }
    .toast-error .toast-icon { color: #ef4444; }
    .toast-warning .toast-icon { color: #FBBF24; }
    .toast-info .toast-icon { color: #60A5FA; }

    .toast-icon {
      flex-shrink: 0;
    }

    .toast-message {
      flex: 1;
      font-size: 14px;
      line-height: 1.4;
    }

    .toast-close {
      flex-shrink: 0;
      color: var(--color-shade-50);
      padding: 4px;
      border-radius: 4px;
      transition: color var(--transition-fast);
    }

    .toast-close:hover {
      color: var(--color-white);
    }

    @keyframes slideIn {
      from { opacity: 0; transform: translateX(32px); }
      to { opacity: 1; transform: translateX(0); }
    }

    @media (max-width: 640px) {
      .toast-container {
        left: 16px;
        right: 16px;
        max-width: none;
      }
    }
  `]
})
export class ToastComponent {
  toastService = inject(ToastService);
}
