import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { tap } from 'rxjs';
import { AbpLocalStorageService, LocalizationPipe } from '@abp/ng.core';
import { ButtonComponent } from '@abp/ng.theme.shared';
import { AbstractAccountSettingsComponent, AbstractAccountSettingsService } from '../../abstracts';
import { AccountIdleSessionService } from '../../services';
import { AccountIdleSettingsDto } from '../../models';
import { eIdleTimeoutMinutes, idleTimeoutMinuteOptions } from '../../enums';

@Component({
  selector: 'abp-account-settings-idle-session',
  templateUrl: './account-settings-idle-session.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: AbstractAccountSettingsService,
      useClass: AccountIdleSessionService,
    },
  ],
  imports: [FormsModule, LocalizationPipe, ButtonComponent],
})
export class AccountSettingsIdleSessionComponent
  extends AbstractAccountSettingsComponent<AccountIdleSettingsDto>
  implements OnInit
{
  protected readonly localStorageService = inject(AbpLocalStorageService);

  isIdleSessionEnabled = signal<boolean>(false);
  timeoutMinute = signal<number>(eIdleTimeoutMinutes.OneHour);

  customIdleTimeout = computed(() => {
    const standardHours = Object.values(eIdleTimeoutMinutes);
    return (
      !standardHours.includes(this.timeoutMinute()) ||
      this.timeoutMinute() === eIdleTimeoutMinutes.CustomIdleTimeoutMinutes
    );
  });

  idleTimeoutMinuteOptions = idleTimeoutMinuteOptions;

  ngOnInit(): void {
    super.ngOnInit();
    this.settings$
      .pipe(
        tap(settings => {
          this.isIdleSessionEnabled.set(settings.enabled);
          this.timeoutMinute.set(settings.idleTimeoutMinutes);
        }),
      )
      .subscribe();
  }

  submit() {
    super.submit({
      enabled: this.isIdleSessionEnabled(),
      idleTimeoutMinutes: this.timeoutMinute(),
    });
    this.localStorageService.setItem(
      'isIdleSessionEnabled',
      this.isIdleSessionEnabled() ? 'true' : 'false',
    );
  }
}
