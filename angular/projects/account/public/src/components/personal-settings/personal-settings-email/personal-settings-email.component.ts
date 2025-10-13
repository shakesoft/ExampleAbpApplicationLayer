import { Component, inject } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import {
  AbstractControl,
  FormGroupDirective,
  UntypedFormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { Observable } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { NgxValidateCoreModule } from '@ngx-validate/core';
import { ProfileDto } from '@volo/abp.ng.account/public/proxy';
import {
  ConfigStateService,
  EnvironmentService,
  LocalizationPipe,
  SubscriptionService,
} from '@abp/ng.core';
import {
  EXTENSIBLE_FORM_VIEW_PROVIDER,
  EXTENSIONS_FORM_PROP,
  EXTENSIONS_FORM_PROP_DATA,
  FormProp,
} from '@abp/ng.components/extensible';
import { ToasterService } from '@abp/ng.theme.shared';
import { AccountService, ManageProfileStateService } from '../../../services';
import { PersonalSettingsVerifyButtonComponent } from '../personal-settings-verify-button/personal-settings-verify-button.component';

@Component({
  selector: 'abp-personal-settings-email',
  templateUrl: './personal-settings-email.component.html',
  viewProviders: [EXTENSIBLE_FORM_VIEW_PROVIDER],
  imports: [
    NgxValidateCoreModule,
    FormsModule,
    ReactiveFormsModule,
    AsyncPipe,
    LocalizationPipe,
    PersonalSettingsVerifyButtonComponent,
  ],
})
export class PersonalSettingsEmailComponent {
  private formProp = inject<FormProp>(EXTENSIONS_FORM_PROP);
  private propData = inject<ProfileDto>(EXTENSIONS_FORM_PROP_DATA);
  private formGroupDirective = inject(FormGroupDirective);
  private manageProfileState = inject(ManageProfileStateService);
  private toasterService = inject(ToasterService);
  private accountService = inject(AccountService);
  private configState = inject(ConfigStateService);
  private environmentService = inject(EnvironmentService);
  protected readonly subscriptionService = inject(SubscriptionService);

  public displayName: string;
  public name: string;
  public id: string;

  public initialValue: string;
  public isValueChanged$: Observable<boolean>;
  public isVerified: boolean;
  public isReadonly$: Observable<boolean>;
  public showEmailVerificationBtn$: Observable<boolean>;
  private formGroup: UntypedFormGroup;
  private formControl: AbstractControl;

  constructor() {
    this.displayName = this.formProp.displayName;
    this.name = this.formProp.name;
    this.id = this.formProp.id;
    this.formGroup = this.formGroupDirective.control;
    this.formControl = this.formGroup.controls[this.name];
    this.initialValue = this.propData.email;

    this.isValueChanged$ = this.formControl.valueChanges.pipe(
      map(value => value !== this.initialValue),
    );
    this.isReadonly$ = this.configState
      .getSetting$('Abp.Identity.User.IsEmailUpdateEnabled')
      .pipe(map(x => x.toLowerCase() !== 'true'));

    this.isVerified = this.propData.emailConfirmed;

    this.showEmailVerificationBtn$ = this.manageProfileState.createStateStream(
      data => !data.hideEmailVerificationBtn,
    );
  }

  get userId(): string {
    return this.configState.getDeep('currentUser.id');
  }

  sendEmailVerificationToken() {
    if (this.formControl.invalid) {
      return;
    }

    const email = this.formControl.value;
    const userId = this.userId;

    const request$ = this.environmentService.getEnvironment$().pipe(
      map(({ application: { baseUrl }, oAuthConfig: { responseType } }) => ({
        appName: responseType === 'code' ? 'MVC' : 'Angular',
        returnUrl: `${baseUrl}/account/login`,
      })),
      switchMap(({ appName, returnUrl }) =>
        this.accountService.sendEmailConfirmationToken({
          appName,
          email,
          userId,
          returnUrl,
        }),
      ),
    );

    this.subscriptionService.addOne(request$, () => {
      this.toasterService.success('AbpAccount::EmailConfirmationSentMessage', '', {
        messageLocalizationParams: [email],
      });
      this.manageProfileState.setHideEmailVerificationBtn(true);
    });
  }
}
