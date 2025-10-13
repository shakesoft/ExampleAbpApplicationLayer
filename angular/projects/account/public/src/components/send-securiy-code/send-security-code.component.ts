import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { AccountService } from '../../services/account.service';
import { SecurityCodeService } from '../../services/security-code.service';
import { NgxValidateCoreModule } from '@ngx-validate/core';
import { RouterLink } from '@angular/router';
import { AutofocusDirective, LocalizationPipe } from '@abp/ng.core';
import { ButtonComponent } from '@abp/ng.theme.shared';

@Component({
  selector: 'abp-send-security-code',
  templateUrl: 'send-security-code.component.html',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NgxValidateCoreModule,
    LocalizationPipe,
    AutofocusDirective,
    ButtonComponent,
    RouterLink,
  ],
})
export class SendSecurityCodeComponent implements OnInit, OnDestroy {
  protected service = inject(SecurityCodeService);
  protected accountService = inject(AccountService);
  protected fb = inject(UntypedFormBuilder);

  providers: string[] = [];
  selectedProvider: string;
  loading: boolean;
  showCodeForm: boolean;

  codeForm = this.fb.group({
    code: [null, [Validators.required]],
  });

  ngOnInit() {
    const { twoFactorToken: token, userId } = this.service.data;

    this.accountService.getTwoFactorProviders({ token, userId }).subscribe(res => {
      this.providers = res;
      this.selectedProvider = res[0];
    });
  }

  ngOnDestroy() {
    this.service.data = null;
  }

  sendTwoFactorCode() {
    if (this.loading) return;
    this.loading = true;

    const { twoFactorToken: token, userId } = this.service.data;
    this.accountService
      .sendTwoFactorCode({
        token,
        userId,
        provider: this.selectedProvider,
      })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe(() => {
        this.showCodeForm = true;
      });
  }

  login() {
    if (this.codeForm.invalid) return;

    const { code } = this.codeForm.value;

    this.loading = true;
    this.service
      .login({ code, provider: this.selectedProvider })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe();
  }
}
