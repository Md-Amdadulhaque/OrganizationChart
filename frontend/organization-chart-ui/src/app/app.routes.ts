import { Routes } from '@angular/router';

import { Dashboard } from './features/dashboard/dashboard';
import { DepartmentList } from './features/departments/department-list/department-list';
import { UserList } from './features/users/user-list/user-list';
import { Import } from './features/import/import';
import { OrganizationChart } from './features/organization-chart/organization-chart';
import { DepartmentForm } from './features/departments/department-form/department-form';
import { UserForm } from './features/users/user-form/user-form';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    component: Dashboard
  },
  {
    path: 'departments',
    component: DepartmentList
  },
  {
    path: 'users',
    component: UserList
  },
  {
    path: 'import',
    component: Import
  },
  {
    path: 'organization-chart',
    component: OrganizationChart
  },
  {
    path: 'departments/create',
    component: DepartmentForm
  },
  {
    path: 'departments/edit/:id',
    component: DepartmentForm
  },
  {
    path: 'users/create',
    component: UserForm
  },
  {
    path: 'users/edit/:id',
    component: UserForm
  }
];