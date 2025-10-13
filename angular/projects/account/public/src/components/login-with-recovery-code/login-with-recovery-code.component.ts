import { Component, inject } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { AutofocusDirective, LocalizationPipe } from '@abp/ng.core';
import { ButtonComponent } from '@abp/ng.theme.shared';
import { RecoveryCodeService } from '../../services';

const { required } = Validators;

@Component({
  selector: 'abp-login-with-recovery-code',
  templateUrl: './login-with-recovery-code.component.html',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    AutofocusDirective,
    LocalizationPipe,
    ButtonComponent,
  ],
})
export class LoginWithRecoveryCodeComponent {
  protected readonly fb = inject(FormBuilder);
  protected readonly recoveryCodeService = inject(RecoveryCodeService);

  protected loading = false;
  protected readonly form = this.fb.group({
    recoveryCode: [null, required],
  });

  login(): void {
    if (this.form.invalid) return;

    this.loading = true;
    this.recoveryCodeService
      .login(this.form.value.recoveryCode)
      .pipe(finalize(() => (this.loading = false)))
      .subscribe();
  }
}
