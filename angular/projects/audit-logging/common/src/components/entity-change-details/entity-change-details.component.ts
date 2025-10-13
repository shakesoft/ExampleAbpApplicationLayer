import { Component, inject, Input } from '@angular/core';
import { NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { LocalizationPipe, LocalizationService, ShortDateTimePipe } from '@abp/ng.core';
import {
  EntityChangeDto,
  EntityChangeType,
  EntityChangeWithUsernameDto,
  EntityPropertyChangeDto,
} from '@volo/abp.ng.audit-logging/proxy';

@Component({
  selector: 'abp-entity-change-details',
  templateUrl: './entity-change-details.component.html',
  imports: [NgbTooltipModule, LocalizationPipe, ShortDateTimePipe],
  styles: [
    `
      th,
      td {
        text-align: left;
        padding: 10px;
        word-wrap: break-word;
      }

      td pre {
        white-space: break-spaces;
        background: inherit;
      }
    `,
  ],
})
export class EntityChangeDetailsComponent {
  @Input() set itemWithUserName(item: EntityChangeWithUsernameDto) {
    this.changeType = EntityChangeType[item.entityChange.changeType];
    this.userName = item.userName;
    this.entityChange = item.entityChange;
  }

  public readonly localization = inject(LocalizationService);

  entityChange: EntityChangeDto;

  userName: string;

  changeType: string;

  getPropColor(propertyChange: EntityPropertyChangeDto) {
    if (this.entityChange.changeType !== EntityChangeType.Updated) {
      return;
    }
    const { originalValue, newValue } = propertyChange;
    return newValue !== originalValue ? 'red' : undefined;
  }

  getEntityChangeTimeParams() {
    if (!this.entityChange.changeTime) {
      return;
    }

    const now = new Date();
    const changeDate = new Date(this.entityChange.changeTime);
    const timeDifference = now.getTime() - changeDate.getTime();

    const diffInMinutes = Math.floor(timeDifference / (1000 * 60));
    const diffInHours = Math.floor(diffInMinutes / 60);
    const diffInDays = Math.floor(diffInHours / 24);

    let timeAgo: string;
    if (diffInDays > 0) {
      timeAgo = this.localization.instant('AbpAuditLogging::DaysAgo', diffInDays.toString());
    } else if (diffInHours > 0) {
      timeAgo = this.localization.instant('AbpAuditLogging::HoursAgo', diffInHours.toString());
    } else {
      timeAgo = this.localization.instant('AbpAuditLogging::MinutesAgo', diffInMinutes.toString());
    }

    const changeTypeLocalized = this.localization.instant('AbpAuditLogging::' + this.changeType);

    return [changeTypeLocalized, timeAgo, this.userName];
  }
}
