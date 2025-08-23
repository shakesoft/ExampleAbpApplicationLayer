import { ABP, eLayoutType } from '@abp/ng.core';

export const ORDER_BASE_ROUTES: ABP.Route[] = [
  {
    path: '/orders',
    iconClass: 'fas fa-file-alt',
    name: '::Menu:Orders',
    layout: eLayoutType.application,
    requiredPolicy: 'ExampleAbpApplicationLayer.Orders',
    breadcrumbText: '::Orders',
  },
];
