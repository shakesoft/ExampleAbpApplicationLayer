import { makeEnvironmentProviders } from '@angular/core';
import { IDLE_SESSION_MODAL_PROVIDER } from './idle-session-modal.provider';
import { ACCOUNT_SETTING_TAB_PROVIDERS } from './';

export function provideAccountAdminConfig() {
  return makeEnvironmentProviders([ACCOUNT_SETTING_TAB_PROVIDERS, IDLE_SESSION_MODAL_PROVIDER]);
}
