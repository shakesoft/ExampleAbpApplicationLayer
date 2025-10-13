import { AbpWindowService, EnvironmentService, RoutesService } from '@abp/ng.core';
import { inject } from '@angular/core';
import { eAccountRouteNames } from '../enums';

export function navigateToManageProfileFactory() {
  const environment = inject(EnvironmentService);
  const windowService = inject(AbpWindowService);
  const routes = inject(RoutesService);

  return () => {
    const { oAuthConfig } = environment.getEnvironment() || {};

    if (oAuthConfig.responseType === 'code') {
      const issuer = environment.getIssuer();
      windowService.open(`${issuer}Account/Manage`, '_blank');
    } else {
      const { path } = routes.find(item => item.name === eAccountRouteNames.ManageProfile);
      windowService.open(path, '_blank');
    }
  };
}

export function navigateToMySecurityLogsFactory() {
  const environment = inject(EnvironmentService);
  const windowService = inject(AbpWindowService);
  const routes = inject(RoutesService);

  return () => {
    const { oAuthConfig } = environment.getEnvironment() || {};

    if (oAuthConfig.responseType === 'code') {
      const issuer = environment.getIssuer();
      windowService.open(`${issuer}Account/SecurityLogs`, '_blank');
    } else {
      const { path } = routes.find(item => item.name === eAccountRouteNames.MySecurityLogs);
      windowService.open(path, '_blank');
    }
  };
}

export function navigateToSessionsFactory() {
  const environment = inject(EnvironmentService);
  const windowService = inject(AbpWindowService);
  const routes = inject(RoutesService);

  return () => {
    const { oAuthConfig } = environment.getEnvironment() || {};

    if (oAuthConfig.responseType === 'code') {
      const issuer = environment.getIssuer();
      windowService.open(`${issuer}Account/Sessions`, '_blank');
    } else {
      const { path } = routes.find(item => item.name === eAccountRouteNames.Sessions);
      windowService.open(path, '_blank');
    }
  };
}
