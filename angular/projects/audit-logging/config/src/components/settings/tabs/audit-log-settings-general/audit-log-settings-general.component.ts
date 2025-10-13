import { Component, ChangeDetectionStrategy } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { LocalizationPipe } from '@abp/ng.core';
import { ButtonComponent } from '@abp/ng.theme.shared';
import { AbstractAuditLogSettingsComponent } from '../../../../abstracts';

@Component({
  selector: 'abp-audit-log-settings-general',
  templateUrl: './audit-log-settings-general.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, LocalizationPipe, DatePipe, ButtonComponent],
})
export class AuditLogSettingsGeneralComponent extends AbstractAuditLogSettingsComponent {}
