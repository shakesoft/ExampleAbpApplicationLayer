import { Component, ChangeDetectionStrategy } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AutofocusDirective, LocalizationPipe } from '@abp/ng.core';
import { ButtonComponent } from '@abp/ng.theme.shared';
import { AccountSettings } from '../../models/account-settings';
import { AccountSettingsService } from '../../services/account-settings.service';
import { AbstractAccountSettingsService } from '../../abstracts/abstract-account-config.service';
import { AbstractAccountSettingsComponent } from '../../abstracts/abstract-account-settings.component';

@Component({
  selector: 'abp-account-settings-general',
  templateUrl: './account-settings-general.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: AbstractAccountSettingsService,
      useClass: AccountSettingsService,
    },
  ],
  imports: [FormsModule, AutofocusDirective, AsyncPipe, LocalizationPipe, ButtonComponent],
})
export class AccountSettingsGeneralComponent extends AbstractAccountSettingsComponent<AccountSettings> {}
