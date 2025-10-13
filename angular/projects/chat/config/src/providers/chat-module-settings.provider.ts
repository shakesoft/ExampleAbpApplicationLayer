import { SettingTabsService } from '@abp/ng.setting-management/config';
import { inject, provideAppInitializer } from '@angular/core';
import { eChatModuleTabNames } from '../enums/chat-module-tab-names';
import { ChatTabComponent } from '../components/chat-tab.component';

export const CHAT_SETTINGS_PROVIDERS = [
  provideAppInitializer(() => {
    configureSettingTabs();
  }),
];

export function configureSettingTabs() {
  const settingtabs = inject(SettingTabsService);
  settingtabs.add([
    {
      name: eChatModuleTabNames.ChatTab,
      order: 100,
      component: ChatTabComponent,
      requiredPolicy: 'Chat.Messaging',
    },
  ]);
}
