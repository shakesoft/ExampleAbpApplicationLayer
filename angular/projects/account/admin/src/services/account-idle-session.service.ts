import { Injectable } from '@angular/core';
import { AbstractAccountSettingsService } from '../abstracts';
import { AccountIdleSettingsDto } from '../models';
import { RestService } from '@abp/ng.core';

@Injectable()
export class AccountIdleSessionService extends AbstractAccountSettingsService<AccountIdleSettingsDto> {
  protected url = '/api/account-admin/settings/idle';

  constructor(restService: RestService) {
    super(restService);
  }
}
