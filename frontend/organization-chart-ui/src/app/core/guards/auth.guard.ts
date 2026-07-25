import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from '../services/auth.service';

/** Blocks a route unless the user is logged in. */
export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isAuthenticated()) {
    return true;
  }

  return router.createUrlTree(['/login']);
};

/** Blocks a route unless the user is an Admin. */
export const adminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isAdmin()) {
    return true;
  }

  // Logged-in non-admins go to the dashboard; anonymous users to login.
  return router.createUrlTree([auth.isAuthenticated() ? '/dashboard' : '/login']);
};
