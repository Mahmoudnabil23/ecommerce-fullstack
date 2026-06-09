import { Component, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-role-home-redirect',
  standalone: true,
  template: '',
})
export class RoleHomeRedirectComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  constructor() {
    this.router.navigateByUrl(this.authService.getDefaultRoute());
  }
}
