import { inject, provideAppInitializer } from '@angular/core';
import { Router } from '@angular/router';
import { NavItemsService } from '@abp/ng.theme.shared';
import { ChatIconComponent } from '../components/chat-icon.component';
import { eChatPolicyNames } from '../enums/policy-names';
import { ChatConfigService } from '../services/chat-config.service';

export const CHAT_NAV_ITEM_PROVIDERS = [
  provideAppInitializer(() => {
    configureNavItems();
  }),
];

export function configureNavItems() {
  const navItems = inject(NavItemsService);
  const router = inject(Router);
  const chatConfigService = inject(ChatConfigService);

  navItems.addItems([
    {
      id: 'Chat.ChatIconComponent',
      name: 'Chat::Feature:ChatGroup',
      requiredPolicy: eChatPolicyNames.Messaging,
      component: ChatIconComponent,
      badge: {
        count: chatConfigService.unreadMessagesCount$,
      },
      order: 99.99,
      icon: 'fas fa-comments fa-lg',
      action: () => {
        router.navigate(['/chat']);
      },
    },
  ]);
}
