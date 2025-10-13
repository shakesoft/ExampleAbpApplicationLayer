import {
  AfterViewInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  inject,
  Injector,
} from '@angular/core';
import { UntypedFormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { NgxValidateCoreModule } from '@ngx-validate/core';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import {
  ListService,
  LocalizationPipe,
  mapEnumToOptions,
  PagedResultDto,
  ShortDateTimePipe,
  TrackByService,
} from '@abp/ng.core';
import { EXTENSIONS_IDENTIFIER, GridActionsComponent } from '@abp/ng.components/extensible';
import {
  DateAdapter,
  EllipsisDirective,
  NgxDatatableDefaultDirective,
  NgxDatatableListDirective,
} from '@abp/ng.theme.shared';
import { DateRangePickerComponent } from '@volo/abp.commercial.ng.ui';
import {
  AuditLogsService,
  EntityChangeDto,
  EntityChangeType,
} from '@volo/abp.ng.audit-logging/proxy';
import { handleExcelExport } from '@volo/abp.ng.audit-logging/common';
import { eAuditLoggingComponents } from '../../enums/components';

@Component({
  selector: 'abp-entity-changes',
  templateUrl: './entity-changes.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    ListService,
    {
      provide: EXTENSIONS_IDENTIFIER,
      useValue: eAuditLoggingComponents.EntityChanges,
    },
    { provide: NgbDateAdapter, useClass: DateAdapter },
  ],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NgxValidateCoreModule,
    NgxDatatableModule,
    EllipsisDirective,
    NgxDatatableDefaultDirective,
    NgxDatatableListDirective,
    LocalizationPipe,
    ShortDateTimePipe,
    DateRangePickerComponent,
    GridActionsComponent,
  ],
})
export class EntityChangesComponent implements AfterViewInit {
  protected readonly fb = inject(UntypedFormBuilder);
  protected readonly auditLogsService = inject(AuditLogsService);
  protected readonly cdRef = inject(ChangeDetectorRef);
  public readonly list = inject(ListService);
  public readonly track = inject(TrackByService);
  public readonly injector = inject(Injector);

  response = { items: [], totalCount: 0 } as PagedResultDto<EntityChangeDto>;
  changeType = EntityChangeType;
  changeTypes = mapEnumToOptions(EntityChangeType);
  form = this.fb.group({
    entityChangeType: [''],
    entityTypeFullName: [''],
    times: [{}],
  });

  get data() {
    return this.response.items;
  }

  get count() {
    return this.response.totalCount;
  }

  ngAfterViewInit() {
    this.hookToQuery();
    this.list.get();
  }

  private hookToQuery() {
    this.list
      .hookToQuery(query => {
        const value = {
          ...this.form.value,
          ...this.form.value.times,
        };
        return this.auditLogsService.getEntityChanges({ ...value, ...query });
      })
      .subscribe(res => {
        this.response = res;
        this.cdRef.markForCheck();
      });
  }

  handleSubmit() {
    this.list.get();
  }

  exportToExcel() {
    handleExcelExport(
      this.auditLogsService.exportEntityChangesToExcel(this.form.value),
      this.injector,
    );
  }
}
