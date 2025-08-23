import { inject, provideAppInitializer } from '@angular/core';
import { ABP, RoutesService } from '@abp/ng.core';
import { PRODUCT_BASE_ROUTES } from './product-base.routes';

export const PRODUCTS_PRODUCT_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routesService = inject(RoutesService);
  const routes: ABP.Route[] = [...PRODUCT_BASE_ROUTES];
  routesService.add(routes);
}
