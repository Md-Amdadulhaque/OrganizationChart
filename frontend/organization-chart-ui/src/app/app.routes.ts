import { Routes } from '@angular/router';

import { Dashboard } from './features/dashboard/dashboard';
import { DepartmentList } from './features/departments/department-list/department-list';
import { UserList } from './features/users/user-list/user-list';
import { Import } from './features/import/import';
import { OrganizationChart } from './features/organization-chart/organization-chart';
import { DepartmentForm } from './features/departments/department-form/department-form';
import { UserForm } from './features/users/user-form/user-form';
import { Login } from './features/auth/login/login';
import { authGuard, adminGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    component: Login
  },
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    component: Dashboard,
    canActivate: [authGuard]
  },
  {
    path: 'departments',
    component: DepartmentList,
    canActivate: [authGuard]
  },
  {
    path: 'users',
    component: UserList,
    canActivate: [authGuard]
  },
  {
    path: 'import',
    component: Import,
    canActivate: [adminGuard]
  },
  {
    path: 'organization-chart',
    component: OrganizationChart,
    canActivate: [authGuard]
  },
  {
    path: 'departments/create',
    component: DepartmentForm,
    canActivate: [adminGuard]
  },
  {
    path: 'departments/edit/:id',
    component: DepartmentForm,
    canActivate: [adminGuard]
  },
  {
    path: 'users/create',
    component: UserForm,
    canActivate: [adminGuard]
  },
  {
    path: 'users/edit/:id',
    component: UserForm,
    canActivate: [adminGuard]
  }
];
