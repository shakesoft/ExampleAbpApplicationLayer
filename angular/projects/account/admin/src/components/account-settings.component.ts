import { AsyncPipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import {
  NgbNav,
  NgbNavItem,
  NgbNavItemRole,
  NgbNavLink,
  NgbNavLinkBase,
  NgbNavContent,
  NgbNavOutlet,
} from '@ng-bootstrap/ng-bootstrap';
import { ConfigStateService, LocalizationPipe, SessionStateService } from '@abp/ng.core';
import { eTwoFactorBehaviour } from '../enums/two-factor-behaviour';
import { AccountCaptchaService } from '../services/account-captcha.service';
import { AccountExternalProviderService } from '../services/account-external-provider.service';
import { AccountSettingsGeneralComponent } from './account-settings-general/account-settings-general.component';
import { AccountSettingsTwoFactorComponent } from './account-settings-two-factor/account-settings-two-factor.component';
import { AccountSettingsCaptchaComponent } from './account-settings-captcha/account-settings-captcha.component';
import { AccountSettingsExternalProviderComponent } from './account-settings-external-provider/account-settings-external-provider.component';
import { AccountSettingsIdleSessionComponent } from './account-settings-idle-session/account-settings-idle-session.component';

@Component({
  selector: 'abp-account-settings',
  templateUrl: './account-settings.component.html',
  providers: [AccountExternalProviderService, AccountCaptchaService],
  imports: [
    NgbNav,
    NgbNavItem,
    NgbNavItemRole,
    NgbNavLink,
    NgbNavLinkBase,
    NgbNavContent,
    AccountSettingsGeneralComponent,
    AccountSettingsTwoFactorComponent,
    AccountSettingsCaptchaComponent,
    AccountSettingsExternalProviderComponent,
    AccountSettingsIdleSessionComponent,
    NgbNavOutlet,
    AsyncPipe,
    LocalizationPipe,
  ],
})
export class AccountSettingsComponent implements OnInit {
  private configStateService = inject(ConfigStateService);
  private sessionStateService = inject(SessionStateService);
  private captchaService = inject(AccountCaptchaService);
  private externalProviderService = inject(AccountExternalProviderService);

  isTwoFactorSettingsEnabled: boolean;
  isExternalProviderEnabled$: Observable<boolean>;
  isExternalProviderExists$: Observable<boolean>;
  isCaptchaEnabled$: Observable<boolean>;
  isTenant: boolean;

  ngOnInit() {
    this.isTwoFactorSettingsEnabled =
      this.configStateService.getFeature('Identity.TwoFactor') ===
      eTwoFactorBehaviour[eTwoFactorBehaviour.Optional];

    this.isExternalProviderExists$ = this.externalProviderService
      .getSettings()
      .pipe(map(data => data?.externalProviders?.length > 0));

    this.isTenant = this.sessionStateService.getTenant()?.isAvailable;

    if (this.isTenant) {
      this.isExternalProviderEnabled$ = this.externalProviderService
        .getSettings()
        .pipe(
          map(result => result.externalProviders.some(settings => settings.enabledForTenantUser)),
        );

      this.isCaptchaEnabled$ = this.captchaService
        .getSettings()
        .pipe(map(result => result.useCaptchaOnLogin || result.useCaptchaOnRegistration));
    } else {
      this.isExternalProviderEnabled$ = of(true);
      this.isCaptchaEnabled$ = of(true);
    }
  }
}
