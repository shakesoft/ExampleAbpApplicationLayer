import { AuthService } from '@abp/ng.core';
import { makeEnvironmentProviders, inject, provideAppInitializer } from '@angular/core';
import { ChatConfigService } from '../services';
import { CHAT_SETTINGS_PROVIDERS, CHAT_ROUTE_PROVIDERS, CHAT_NAV_ITEM_PROVIDERS } from './';

export function provideChatConfig() {
  return makeEnvironmentProviders([
    CHAT_ROUTE_PROVIDERS,
    CHAT_NAV_ITEM_PROVIDERS,
    CHAT_SETTINGS_PROVIDERS,
    provideAppInitializer(() => {
      inject(ChatConfigService);
      inject(AuthService);
    }),
  ]);
}
