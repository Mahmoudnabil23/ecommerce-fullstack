import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';
import { AuthService } from '../services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toastService = inject(ToastService);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error) => {
      if (error.status === 401) {
        // If we get a 401 and it's not a login/refresh request, log out
        if (!req.url.includes('/auth/login') && !req.url.includes('/auth/refresh-token') && !req.url.includes('/auth/logout')) {
          authService.logout(false);
          toastService.error('Session expired. Please login again.');
        }
      } else if (error.status === 403) {
        toastService.error('You do not have permission to perform this action.');
      } else if (error.status === 0) {
        toastService.error('Unable to connect to the server. Please check your connection.');
      }

      return throwError(() => error);
    })
  );
};
