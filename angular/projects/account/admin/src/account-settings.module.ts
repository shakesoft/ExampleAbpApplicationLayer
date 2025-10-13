import { NgModule } from '@angular/core';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxValidateCoreModule } from '@ngx-validate/core';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { AbstractAccountSettingsComponent } from './abstracts/abstract-account-settings.component';
import { AccountSettingsGeneralComponent } from './components/account-settings-general/account-settings-general.component';
import { AccountSettingsTwoFactorComponent } from './components/account-settings-two-factor/account-settings-two-factor.component';
import { AccountSettingsComponent } from './components/account-settings.component';
import { AccountSettingsCaptchaComponent } from './components/account-settings-captcha/account-settings-captcha.component';
import { AccountSettingsExternalProviderComponent } from './components/account-settings-external-provider/account-settings-external-provider.component';
import { IdleSessionModalComponent, AccountSettingsIdleSessionComponent } from './components';

const components = [
  AbstractAccountSettingsComponent,
  AccountSettingsComponent,
  AccountSettingsGeneralComponent,
  AccountSettingsTwoFactorComponent,
  AccountSettingsCaptchaComponent,
  AccountSettingsExternalProviderComponent,
  AccountSettingsIdleSessionComponent,
];

@NgModule({
  imports: [
    CoreModule,
    ThemeSharedModule,
    NgbNavModule,
    NgxValidateCoreModule,
    IdleSessionModalComponent,
    ...components,
  ],
  exports: [...components, IdleSessionModalComponent],
})
export class AccountSettingsModule {}
