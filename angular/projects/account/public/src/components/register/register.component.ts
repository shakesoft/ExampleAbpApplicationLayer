import {
  AfterViewInit,
  Component,
  ElementRef,
  inject,
  Injector,
  OnInit,
  ViewChild,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxValidateCoreModule } from '@ngx-validate/core';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { of, throwError } from 'rxjs';
import { catchError, finalize, switchMap } from 'rxjs/operators';
import {
  AuthService,
  AutofocusDirective,
  ConfigStateService,
  LocalizationPipe,
  ShowPasswordDirective,
  TrackCapsLockDirective,
} from '@abp/ng.core';
import { ButtonComponent, getPasswordValidators, ToasterService } from '@abp/ng.theme.shared';
import {
  AccountService,
  RecaptchaService,
  PasswordComplexityIndicatorService,
} from '../../services';
import { RECAPTCHA_STRATEGY } from '../../strategies/recaptcha.strategy';
import { getRedirectUrl } from '../../utils/auth-utils';
import { ProgressBarStats } from '../../models/password-complexity';
import { RegisterFormModel } from '../../models/registerFormModel';
import { PasswordComplexityIndicatorComponent } from '../password-complexity-indicator/password-complexity-indicator.component';

const { maxLength, required, email } = Validators;

@Component({
  selector: 'abp-register',
  templateUrl: './register.component.html',
  providers: [RecaptchaService],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NgxValidateCoreModule,
    AutofocusDirective,
    ShowPasswordDirective,
    TrackCapsLockDirective,
    LocalizationPipe,
    PasswordComplexityIndicatorComponent,
    ButtonComponent,
    NgbTooltip,
    RouterLink,
  ],
})
export class RegisterComponent implements OnInit, AfterViewInit {
  protected injector = inject(Injector);

  @ViewChild('recaptcha', { static: false })
  recaptchaRef: ElementRef<HTMLDivElement>;

  form: FormGroup<RegisterFormModel>;

  inProgress: boolean;

  isSelfRegistrationEnabled = true;

  showPassword = false;
  capsLock = false;
  progressBar: ProgressBarStats;

  protected fb: FormBuilder;
  protected accountService: AccountService;
  protected toasterService: ToasterService;
  protected configState: ConfigStateService;
  protected authService: AuthService;
  protected recaptchaService: RecaptchaService;
  protected passwordComplexityService: PasswordComplexityIndicatorService;

  constructor() {
    this.fb = this.injector.get(FormBuilder);
    this.accountService = this.injector.get(AccountService);
    this.toasterService = this.injector.get(ToasterService);
    this.configState = this.injector.get(ConfigStateService);
    this.authService = this.injector.get(AuthService);
    this.recaptchaService = this.injector.get(RecaptchaService);
    this.passwordComplexityService = this.injector.get(PasswordComplexityIndicatorService);
  }

  ngOnInit() {
    this.isSelfRegistrationEnabled =
      (
        (this.configState.getSetting('Abp.Account.IsSelfRegistrationEnabled') as string) || ''
      ).toLowerCase() !== 'false';
    if (!this.isSelfRegistrationEnabled) {
      this.toasterService.warn(
        {
          key: 'AbpAccount::SelfRegistrationDisabledMessage',
          defaultValue: 'Self registration is disabled.',
        },
        null,
        { life: 10000 },
      );
      return;
    }

    this.form = this.fb.group({
      username: ['', [required, maxLength(255)]],
      password: ['', [required, ...getPasswordValidators(this.injector)]],
      email: ['', [required, email]],
    });
  }

  ngAfterViewInit() {
    this.recaptchaService.setStrategy(
      RECAPTCHA_STRATEGY.Register(this.configState, this.recaptchaRef.nativeElement),
    );
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.inProgress = true;

    const newUser = {
      userName: this.form.get('username').value,
      password: this.form.get('password').value,
      emailAddress: this.form.get('email').value,
      appName: 'Angular',
    };

    (this.recaptchaService.isEnabled ? this.recaptchaService.getVerificationToken() : of(undefined))
      .pipe(
        switchMap(captchaResponse =>
          this.accountService.register({ ...newUser, captchaResponse }).pipe(
            switchMap(() =>
              this.authService.login({
                username: newUser.userName,
                password: newUser.password,
                redirectUrl: getRedirectUrl(this.injector) || '/',
              }),
            ),
            catchError(error => {
              this.recaptchaService.reset();
              this.toasterService.error(
                error.error?.error_description ||
                  error.error?.error?.message ||
                  'AbpAccount::DefaultErrorMessage',
                null,
                { life: 7000 },
              );
              return throwError(() => error);
            }),
          ),
        ),

        finalize(() => (this.inProgress = false)),
      )

      .subscribe();
  }

  get password(): string {
    return this.form.value.password;
  }

  validatePassword() {
    this.progressBar = this.passwordComplexityService.validatePassword(this.password);
  }
}
