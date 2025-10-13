import { Component, inject } from '@angular/core';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { LocalizationPipe, SessionStateService } from '@abp/ng.core';
import { AuditLogSettingsGeneralComponent, AuditLogSettingsGlobalComponent } from './tabs';

@Component({
  selector: 'abp-audit-log-settings',
  templateUrl: './audit-log-settings.component.html',
  imports: [
    NgbNavModule,
    LocalizationPipe,
    AuditLogSettingsGeneralComponent,
    AuditLogSettingsGlobalComponent,
  ],
})
export class AuditLogSettingsComponent {
  protected readonly sessionStateService = inject(SessionStateService);

  isTenant = this.sessionStateService.getTenant()?.isAvailable;
}
