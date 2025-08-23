import { ABP, eLayoutType } from '@abp/ng.core';

export const PRODUCT_BASE_ROUTES: ABP.Route[] = [
  {
    path: '/products',
    iconClass: 'fas fa-file-alt',
    name: '::Menu:Products',
    layout: eLayoutType.application,
    requiredPolicy: 'ExampleAbpApplicationLayer.Products',
    breadcrumbText: '::Products',
  },
];
