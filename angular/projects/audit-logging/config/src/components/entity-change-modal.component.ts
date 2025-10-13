import { ChangeDetectionStrategy, ChangeDetectorRef, Component, inject } from '@angular/core';
import { LocalizationPipe, TrackByService } from '@abp/ng.core';
import { ModalCloseDirective, ModalComponent } from '@abp/ng.theme.shared';
import { EntityChangeWithUsernameDto } from '@volo/abp.ng.audit-logging/proxy';
import { EntityChangeDetailsComponent } from '@volo/abp.ng.audit-logging/common';

@Component({
  selector: 'abp-entity-change-modal',
  templateUrl: './entity-change-modal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ModalCloseDirective, LocalizationPipe, EntityChangeDetailsComponent, ModalComponent],
})
export class EntityChangeModalComponent {
  public readonly cdRef = inject(ChangeDetectorRef);
  public readonly track = inject(TrackByService);

  history: EntityChangeWithUsernameDto[] = [];

  entityId: string;

  entityTypeFullName: string;

  visible = false;
}
