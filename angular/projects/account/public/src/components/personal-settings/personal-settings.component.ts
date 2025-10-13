import { Component, EmbeddedViewRef, inject, Injector } from '@angular/core';
import { FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxValidateCoreModule } from '@ngx-validate/core';
import { Observable } from 'rxjs';
import { filter, tap } from 'rxjs/operators';
import {
  AuthService,
  ConfigStateService,
  LocalizationPipe,
  SubscriptionService,
} from '@abp/ng.core';
import { ButtonComponent, ConfirmationService, ToasterService } from '@abp/ng.theme.shared';
import {
  ExtensibleFormComponent,
  EXTENSIONS_IDENTIFIER,
  FormPropData,
  generateFormFromProps,
} from '@abp/ng.components/extensible';
import { ProfileDto, ProfileService } from '@volo/abp.ng.account/public/proxy';
import { ManageProfileStateService } from '../../services/manage-profile-state.service';
import { eAccountComponents } from '../../enums';

@Component({
  selector: 'abp-personal-settings-form',
  templateUrl: './personal-settings.component.html',
  providers: [
    SubscriptionService,
    {
      provide: EXTENSIONS_IDENTIFIER,
      useValue: eAccountComponents.PersonalSettings,
    },
  ],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NgxValidateCoreModule,
    LocalizationPipe,
    ExtensibleFormComponent,
    ButtonComponent,
  ],
})
export class PersonalSettingsComponent {
  readonly #injector = inject(Injector);
  protected readonly toasterService = inject(ToasterService);
  protected readonly confirmationService = inject(ConfirmationService);
  protected readonly subscription = inject(SubscriptionService);
  protected readonly manageProfileState = inject(ManageProfileStateService);
  protected readonly profileService = inject(ProfileService);
  protected readonly configState = inject(ConfigStateService);
  protected readonly authService = inject(AuthService);

  storedProfile: ProfileDto;

  profile$: Observable<ProfileDto> = this.manageProfileState.getProfile$();

  modalBusy = false;
  modalRef: EmbeddedViewRef<any>;

  form: FormGroup<any>;

  token = '';

  isEmailUpdateEnabled = true;

  isUserNameUpdateEnabled = true;

  buildForm = (profile: ProfileDto) => {
    const data = new FormPropData(this.#injector, profile);
    this.form = generateFormFromProps(data);
  };

  constructor() {
    this.profile$
      .pipe(
        filter<ProfileDto>(Boolean),
        tap(profile => (this.storedProfile = profile)),
      )
      .subscribe(this.buildForm);
  }

  submit() {
    if (this.form.invalid) return;

    const isRefreshTokenExists = this.authService.getRefreshToken();
    const { phoneNumberConfirmed, ...profile } = this.form.value;

    this.profileService.update(profile).subscribe(res => {
      this.manageProfileState.setProfile(res);
      this.configState.refreshAppState();
      this.toasterService.success('AbpAccount::PersonalSettingsSaved', '', { life: 5000 });

      if (isRefreshTokenExists) {
        return this.authService.refreshToken();
      }
    });
  }
}
