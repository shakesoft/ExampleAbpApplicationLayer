import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { LocalizationPipe, MultiTenancyService } from '@abp/ng.core';
import { AccountService } from '../../services/account.service';

export interface EmailConfirmationInput {
  confirmationToken: string;
  userId: string;
}

@Component({
  selector: 'abp-email-confirmation',
  template: `
    @if (confirmed) {
      <p>
        {{ 'AbpAccount::YourEmailAddressIsSuccessfullyConfirmed' | abpLocalization }}
      </p>
    }

    @if (notValid) {
      <p class="text-danger font-weight-bold">
        {{ 'AbpUi::ValidationErrorMessage' | abpLocalization }}
      </p>
    }

    <a role="button" class="btn btn-primary" [routerLink]="['/account/login']">{{
      'AbpAccount::Login' | abpLocalization
    }}</a>
  `,
  imports: [RouterLink, LocalizationPipe],
})
export class EmailConfirmationComponent implements OnInit, OnDestroy {
  private multiTenancy = inject(MultiTenancyService);
  private accountService = inject(AccountService);
  private route = inject(ActivatedRoute);

  confirmed: boolean;

  notValid: boolean;

  private resetTenantBox = () => {};

  ngOnInit() {
    const { isTenantBoxVisible } = this.multiTenancy;
    this.resetTenantBox = () => (this.multiTenancy.isTenantBoxVisible = isTenantBoxVisible);

    // throws ExpressionChangedAfterItHasBeenCheckedError without setTimeout
    setTimeout(() => (this.multiTenancy.isTenantBoxVisible = false), 0);

    this.confirmation();
  }

  ngOnDestroy() {
    this.resetTenantBox();
  }

  confirmation() {
    const { userId, confirmationToken: token } = this.route.snapshot
      .queryParams as EmailConfirmationInput;

    if (!userId || !token) {
      this.notValid = true;
      return;
    }

    this.accountService.confirmEmail({ userId, token }).subscribe(() => (this.confirmed = true));
  }
}
