import { eLayoutType, RoutesService } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';
import { eChatPolicyNames } from '../enums/policy-names';
import { eChatRouteNames } from '../enums/route-names';

export const CHAT_ROUTE_PROVIDERS = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

export function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
    {
      path: '/chat',
      name: eChatRouteNames.Chat,
      requiredPolicy: eChatPolicyNames.Messaging,
      layout: eLayoutType.application,
      iconClass: 'fa fa-comments',
      breadcrumbText: 'Chat::Menu:Chat',
      invisible: true,
    },
  ]);
}
