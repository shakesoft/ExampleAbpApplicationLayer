import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgTemplateOutlet, AsyncPipe } from '@angular/common';
import { map } from 'rxjs/operators';
import { AutofocusDirective, LocalizationPipe } from '@abp/ng.core';
import { ButtonComponent } from '@abp/ng.theme.shared';
import { AbstractAccountSettingsService, AbstractAccountSettingsComponent } from '../../abstracts';
import { AccountExternalProviderService } from '../../services/account-external-provider.service';

import {
  AccountExternalProviderSettings,
  AccountExternalProviderSetting,
} from '../../models/account-settings';

@Component({
  selector: 'abp-account-settings-external-provider',
  templateUrl: './account-settings-external-provider.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: AbstractAccountSettingsService,
      useClass: AccountExternalProviderService,
    },
  ],
  imports: [
    FormsModule,
    AutofocusDirective,
    AsyncPipe,
    LocalizationPipe,
    ButtonComponent,
    NgTemplateOutlet,
  ],
})
export class AccountSettingsExternalProviderComponent
  extends AbstractAccountSettingsComponent<AccountExternalProviderSettings>
  implements OnInit
{
  mapInitialTenantSettings = (result: AccountExternalProviderSettings) => ({
    ...result,
    externalProviders: result.externalProviders
      .filter(setting => (this.isTenant ? setting.enabledForTenantUser : setting.enabled))
      .map(this.setUseHostSettingsOf),
  });

  ngOnInit() {
    if (this.isTenant) {
      this.settings$ = this.service.getSettings().pipe(map(this.mapInitialTenantSettings));
    } else {
      super.ngOnInit();
    }
  }

  mapTenantSettingsForSubmit(newSettings: AccountExternalProviderSettings) {
    return {
      ...newSettings,
      externalProviders: newSettings.externalProviders.map(this.clearPropertyValues),
    };
  }

  private clearPropertyValues(setting: AccountExternalProviderSetting) {
    if (!setting.useCustomSettings) {
      setting.properties.forEach(prop => (prop.value = ''));
      setting.secretProperties.forEach(prop => (prop.value = ''));
    }

    const { useHostSettings, ...mappedSetting } = setting;
    return mappedSetting;
  }

  private setUseHostSettingsOf(setting: AccountExternalProviderSetting) {
    const useHostSettings = !(
      setting.properties.some(prop => prop.value) ||
      setting.secretProperties.some(prop => prop.value)
    );
    return {
      ...setting,
      useHostSettings,
    };
  }
}
