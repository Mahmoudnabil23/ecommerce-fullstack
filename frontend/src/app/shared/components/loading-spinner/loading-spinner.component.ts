import { Component } from '@angular/core';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  template: `
    <div class="spinner-wrapper">
      <div class="spinner"></div>
    </div>
  `,
  styles: [`
    .spinner-wrapper {
      display: flex;
      align-items: center;
      justify-content: center;
      padding: var(--space-10) 0;
    }

    .spinner {
      width: 40px;
      height: 40px;
      border: 3px solid var(--color-shade-70);
      border-top-color: var(--color-neon-green);
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
    }
  `]
})
export class LoadingSpinnerComponent {}
