import { inject, provideAppInitializer } from '@angular/core';
import { IdleSessionService } from '@volo/abp.ng.account/admin';

export const IDLE_SESSION_MODAL_PROVIDER = [
  provideAppInitializer(() => {
    const idleSessionService = inject(IdleSessionService);
    idleSessionService.renderTimeoutModal();
  }),
];
