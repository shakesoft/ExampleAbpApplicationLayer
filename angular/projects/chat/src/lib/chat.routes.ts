import { Routes } from '@angular/router';
import {
  RouterOutletComponent,
  authGuard,
  permissionGuard,
  ReplaceableRouteContainerComponent,
  ReplaceableComponents,
} from '@abp/ng.core';
import { ChatComponent } from './components';

export function createRoutes(): Routes {
  return [
    {
      path: '',
      component: RouterOutletComponent,
      canActivate: [authGuard, permissionGuard],
      children: [
        {
          path: '',
          component: ReplaceableRouteContainerComponent,
          data: {
            requiredPolicy: 'Chat.Messaging',
            replaceableComponent: {
              defaultComponent: ChatComponent,
              key: 'Chat.ChatComponent',
            } as ReplaceableComponents.RouteData<ChatComponent>,
            title: 'Chat::Menu:Chat',
          },
        },
      ],
    },
  ];
}
