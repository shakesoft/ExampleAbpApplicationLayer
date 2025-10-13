import { Component, OnInit, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { ListService, LocalizationPipe, PagedResultDto } from '@abp/ng.core';
import { ExtensibleTableComponent, EXTENSIONS_IDENTIFIER } from '@abp/ng.components/extensible';
import { PageComponent } from '@abp/ng.components/page';
import { DateAdapter } from '@abp/ng.theme.shared';
import { DatetimePickerComponent } from '@volo/abp.commercial.ng.ui';
import { AccountService, Volo } from '@volo/abp.ng.account/public/proxy';
import { eAccountComponents } from '../../enums/components';

@Component({
  selector: 'abp-my-security-logs',
  templateUrl: './my-security-logs.component.html',
  providers: [
    ListService,
    {
      provide: EXTENSIONS_IDENTIFIER,
      useValue: eAccountComponents.MySecurityLogs,
    },
    { provide: NgbDateAdapter, useClass: DateAdapter },
    DatePipe,
  ],
  imports: [
    FormsModule,
    LocalizationPipe,
    DatetimePickerComponent,
    ExtensibleTableComponent,
    PageComponent,
  ],
})
export class MySecurityLogsComponent implements OnInit {
  protected readonly service = inject(AccountService);
  public readonly list = inject(ListService);

  data: PagedResultDto<Volo.Abp.Identity.IdentitySecurityLogDto> = { items: [], totalCount: 0 };

  filter = {} as Partial<Volo.Abp.Identity.GetIdentitySecurityLogListInput>;

  ngOnInit(): void {
    this.hookToQuery();
  }

  private hookToQuery() {
    this.list
      .hookToQuery(query =>
        this.service.getSecurityLogList({
          ...query,
          ...this.filter,
        }),
      )
      .subscribe(res => (this.data = res));
  }
}
