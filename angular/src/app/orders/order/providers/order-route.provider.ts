import { inject, provideAppInitializer } from '@angular/core';
import { ABP, RoutesService } from '@abp/ng.core';
import { ORDER_BASE_ROUTES } from './order-base.routes';

export const ORDERS_ORDER_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routesService = inject(RoutesService);
  const routes: ABP.Route[] = [...ORDER_BASE_ROUTES];
  routesService.add(routes);
}
