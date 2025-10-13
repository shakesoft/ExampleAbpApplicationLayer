import { Component, inject, OnInit } from '@angular/core';
import { FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgClass } from '@angular/common';
import { NgxValidateCoreModule } from '@ngx-validate/core';
import { LocalizationPipe, ShowPasswordDirective, SubscriptionService } from '@abp/ng.core';
import { ButtonComponent } from '@abp/ng.theme.shared';
import { ChangePasswordService } from '../../services/change-password.service';
import { PasswordComplexityIndicatorService } from '../../services/password-complexity-indicator.service';
import { ProgressBarStats } from '../../models/password-complexity';
import { ChangePasswordFormModel } from '../../models/changePasswordFormModel';
import { PasswordComplexityIndicatorComponent } from '../password-complexity-indicator/password-complexity-indicator.component';

@Component({
  selector: 'abp-refresh-password-form',
  templateUrl: './refresh-password.component.html',
  exportAs: 'abpRefreshPasswordForm',
  providers: [SubscriptionService],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NgxValidateCoreModule,
    ShowPasswordDirective,
    LocalizationPipe,
    PasswordComplexityIndicatorComponent,
    ButtonComponent,
    NgClass,
  ],
})
export class RefreshPasswordComponent implements OnInit {
  private readonly passwordComplexityService = inject(PasswordComplexityIndicatorService);
  private readonly service = inject(ChangePasswordService);
  protected readonly subscription = inject(SubscriptionService);
  form: FormGroup<ChangePasswordFormModel>;
  progressBar: ProgressBarStats;
  showCurrentPassword = false;
  showNewPassword = false;
  showConfirmPassword = false;
  mapErrorsFn = this.service.MapErrorsFnFactory();

  ngOnInit(): void {
    this.form = this.service.buildForm();
  }

  onSuccess() {
    const sub = this.service.redirectToReturnUrl();
    this.subscription.addOne(sub);
  }

  onSubmit() {
    if (this.form.invalid) return;
    const input = this.form.value;
    const sub = this.service.changePasswordAndLogin({
      currentPassword: input.currentPassword,
      newPassword: input.newPassword,
    });
    this.subscription.addOne(
      sub,
      () => this.onSuccess(),
      e => this.service.showErrorMessage(e),
    );
  }

  get newPassword(): string {
    return this.form.get('newPassword').value;
  }

  validatePassword() {
    this.progressBar = this.passwordComplexityService.validatePassword(this.newPassword);
  }
}
