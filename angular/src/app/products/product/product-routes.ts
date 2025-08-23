import { Routes } from '@angular/router';
import { authGuard, permissionGuard } from '@abp/ng.core';

export const PRODUCT_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => {
      return import('./components/product.component').then(c => c.ProductComponent);
    },
    canActivate: [authGuard, permissionGuard],
  },
];
