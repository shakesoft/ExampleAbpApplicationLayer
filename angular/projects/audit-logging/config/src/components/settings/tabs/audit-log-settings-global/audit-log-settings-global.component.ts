import { Component, ChangeDetectionStrategy } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { LocalizationPipe } from '@abp/ng.core';
import { ButtonComponent } from '@abp/ng.theme.shared';
import { AbstractAuditLogSettingsComponent } from '../../../../abstracts';

@Component({
  selector: 'abp-audit-log-settings-global',
  templateUrl: './audit-log-settings-global.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, LocalizationPipe, DatePipe, ButtonComponent],
})
export class AuditLogSettingsGlobalComponent extends AbstractAuditLogSettingsComponent {
  globalTab = true;
}
