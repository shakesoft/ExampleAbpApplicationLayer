import { inject, provideAppInitializer } from '@angular/core';
import { SettingTabsService } from '@abp/ng.setting-management/config';
import { eAuditLoggingSettingTabNames } from '../enums/setting-tab-names';
import { AuditLogSettingsComponent } from '../components/settings';

export const AUDIT_LOGGING_SETTING_TAB_PROVIDERS = [
  provideAppInitializer(() => {
    configureSettingTabs();
  }),
];

export function configureSettingTabs() {
  const settingTabsService = inject(SettingTabsService);
  settingTabsService.add([
    {
      order: 100,
      name: eAuditLoggingSettingTabNames.AuditLogging,
      requiredPolicy: 'AuditLogging.AuditLogs.SettingManagement',
      component: AuditLogSettingsComponent,
    },
  ]);
}
