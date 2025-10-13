import { Component, Injector, OnInit, inject } from '@angular/core';
import { NgClass, JsonPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { trigger, transition, useAnimation } from '@angular/animations';
import {
  NgbDateAdapter,
  NgbNav,
  NgbNavItem,
  NgbNavItemRole,
  NgbNavLink,
  NgbNavLinkBase,
  NgbNavContent,
  NgbNavOutlet,
} from '@ng-bootstrap/ng-bootstrap';
import { take } from 'rxjs/operators';
import { ListService, LocalizationPipe, PagedResultDto, ShortDateTimePipe } from '@abp/ng.core';
import { ExtensibleTableComponent, EXTENSIONS_IDENTIFIER } from '@abp/ng.components/extensible';
import { PageComponent } from '@abp/ng.components/page';
import { DateAdapter, fadeIn, ModalCloseDirective, ModalComponent } from '@abp/ng.theme.shared';
import { DatetimePickerComponent } from '@volo/abp.commercial.ng.ui';
import {
  AuditLogDto,
  AuditLogsService,
  EntityChangeDto,
  EntityChangeWithUsernameDto,
  GetAuditLogListDto,
} from '@volo/abp.ng.audit-logging/proxy';
import { EntityChangeDetailsComponent, handleExcelExport } from '@volo/abp.ng.audit-logging/common';
import { HTTP_METHODS, HTTP_STATUS_CODES } from '../../constants/http';
import { eAuditLoggingComponents } from '../../enums/components';
import { EntityChangesComponent } from '../entity-change/entity-changes.component';

@Component({
  selector: 'abp-audit-logs',
  templateUrl: './audit-logs.component.html',
  animations: [trigger('fadeIn', [transition('* <=> *', useAnimation(fadeIn))])],
  providers: [
    ListService,
    {
      provide: EXTENSIONS_IDENTIFIER,
      useValue: eAuditLoggingComponents.AuditLogs,
    },
    { provide: NgbDateAdapter, useClass: DateAdapter },
  ],
  imports: [
    FormsModule,
    NgbNav,
    NgbNavItem,
    NgbNavItemRole,
    NgbNavLink,
    NgbNavLinkBase,
    NgbNavContent,
    NgbNavOutlet,
    NgClass,
    ModalCloseDirective,
    JsonPipe,
    LocalizationPipe,
    ShortDateTimePipe,
    DatetimePickerComponent,
    ExtensibleTableComponent,
    EntityChangesComponent,
    EntityChangeDetailsComponent,
    PageComponent,
    ModalComponent,
  ],
})
export class AuditLogsComponent implements OnInit {
  protected readonly service = inject(AuditLogsService);
  protected readonly injector = inject(Injector);
  protected readonly list = inject(ListService<GetAuditLogListDto>);

  data: PagedResultDto<AuditLogDto> = { items: [], totalCount: 0 };

  selected = {} as AuditLogDto;

  pageQuery = {
    maxResultCount: 10,
    skipCount: 0,
    httpMethod: null,
    httpStatusCode: null,
    hasException: null,
  } as GetAuditLogListDto;

  httpMethods = HTTP_METHODS;

  httpStatusCodes = HTTP_STATUS_CODES;

  modalVisible = false;

  sortOrder = '';

  sortKey = '';

  selectedTab = 'audit-logs';

  protected hookToQuery() {
    this.list
      .hookToQuery(query =>
        this.service.getList({
          ...this.pageQuery,
          ...(this.pageQuery.minExecutionDuration === null && {
            minExecutionDuration: undefined,
          }),
          ...(this.pageQuery.maxExecutionDuration === null && {
            maxExecutionDuration: undefined,
          }),
          ...query,
        }),
      )
      .subscribe(res => (this.data = res));
  }

  ngOnInit() {
    this.hookToQuery();
  }

  openModal(id: string) {
    this.service
      .get(id)
      .pipe(take(1))
      .subscribe(log => {
        this.selected = log;
        this.modalVisible = true;
      });
  }

  httpCodeClass(httpStatusCode: number): string {
    switch (httpStatusCode?.toString()[0]) {
      case '2':
        return 'bg-success';
      case '3':
        return 'bg-warning';
      case '4':
      case '5':
        return 'bg-danger';
      default:
        return 'bg-light';
    }
  }

  httpMethodClass(httpMethod: string): string {
    switch (httpMethod) {
      case 'GET':
        return 'bg-info';
      case 'POST':
        return 'bg-success';
      case 'DELETE':
        return 'bg-danger';
      case 'PUT':
        return 'bg-warning';
      default:
        return '';
    }
  }

  getEntityChangeDetails(change: EntityChangeDto): EntityChangeWithUsernameDto {
    return {
      entityChange: change,
      userName: this.selected.userName,
    };
  }

  exportToExcel() {
    handleExcelExport(this.service.exportToExcel(this.pageQuery), this.injector);
  }
}
