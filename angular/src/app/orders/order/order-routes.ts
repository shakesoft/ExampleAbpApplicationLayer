import { Routes } from '@angular/router';
import { authGuard, permissionGuard } from '@abp/ng.core';

export const ORDER_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => {
      return import('./components/order.component').then(c => c.OrderComponent);
    },
    canActivate: [authGuard, permissionGuard],
  },
];
