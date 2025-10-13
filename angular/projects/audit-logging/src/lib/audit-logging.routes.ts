import { Provider } from '@angular/core';
import { Routes } from '@angular/router';
import {
  RouterOutletComponent,
  authGuard,
  permissionGuard,
  ReplaceableRouteContainerComponent,
  ReplaceableComponents,
} from '@abp/ng.core';
import { AuditLogsComponent } from './components';
import { eAuditLoggingComponents } from './enums';
import { auditLoggingGuard } from './guards';
import { AuditLoggingConfigOptions } from './models';
import { auditLoggingExtensionsResolver } from './resolvers';
import {
  AUDIT_LOGGING_ENTITY_ACTION_CONTRIBUTORS,
  AUDIT_LOGGING_TOOLBAR_ACTION_CONTRIBUTORS,
  AUDIT_LOGGING_ENTITY_PROP_CONTRIBUTORS,
} from './tokens';

export function createRoutes(config: AuditLoggingConfigOptions = {}): Routes {
  return [
    {
      path: '',
      component: RouterOutletComponent,
      providers: provideAuditLoggingContributors(config),
      canActivate: [authGuard, permissionGuard, auditLoggingGuard],
      resolve: [auditLoggingExtensionsResolver],
      children: [
        {
          path: '',
          component: ReplaceableRouteContainerComponent,
          data: {
            requiredPolicy: 'AuditLogging.AuditLogs',
            replaceableComponent: {
              key: eAuditLoggingComponents.AuditLogs,
              defaultComponent: AuditLogsComponent,
            } as ReplaceableComponents.RouteData<AuditLogsComponent>,
          },
          title: 'AbpAuditLogging::Menu:AuditLogging',
        },
      ],
    },
  ];
}

function provideAuditLoggingContributors(options: AuditLoggingConfigOptions = {}): Provider[] {
  return [
    {
      provide: AUDIT_LOGGING_ENTITY_ACTION_CONTRIBUTORS,
      useValue: options.entityActionContributors,
    },
    {
      provide: AUDIT_LOGGING_TOOLBAR_ACTION_CONTRIBUTORS,
      useValue: options.toolbarActionContributors,
    },
    {
      provide: AUDIT_LOGGING_ENTITY_PROP_CONTRIBUTORS,
      useValue: options.entityPropContributors,
    },
  ];
}
