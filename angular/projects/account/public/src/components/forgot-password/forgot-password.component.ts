import { Component, inject } from '@angular/core';
import {
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxValidateCoreModule } from '@ngx-validate/core';
import { finalize } from 'rxjs/operators';
import { AutofocusDirective, LocalizationPipe } from '@abp/ng.core';
import { ButtonComponent } from '@abp/ng.theme.shared';
import { AccountService } from '../../services/account.service';

const { required, email } = Validators;

@Component({
  selector: 'abp-forgot-password',
  templateUrl: './forgot-password.component.html',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NgxValidateCoreModule,
    AutofocusDirective,
    LocalizationPipe,
    ButtonComponent,
    RouterLink,
  ],
})
export class ForgotPasswordComponent {
  private fb = inject(UntypedFormBuilder);
  private accountService = inject(AccountService);

  form: UntypedFormGroup;

  inProgress: boolean;

  isEmailSent = false;

  constructor() {
    this.form = this.fb.group({
      email: ['', [required, email]],
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.inProgress = true;

    this.accountService
      .sendPasswordResetCode({ email: this.form.get('email').value, appName: 'Angular' })
      .pipe(finalize(() => (this.inProgress = false)))
      .subscribe(() => {
        this.isEmailSent = true;
      });
  }
}
