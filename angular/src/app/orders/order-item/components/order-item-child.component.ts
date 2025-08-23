import { ChangeDetectionStrategy, Component } from '@angular/core';
import {
  NgbDateAdapter,
  NgbCollapseModule,
  NgbDatepickerModule,
  NgbDropdownModule,
} from '@ng-bootstrap/ng-bootstrap';
import { NgxValidateCoreModule } from '@ngx-validate/core';
import { NgxDatatableModule, SelectionType } from '@swimlane/ngx-datatable';
import { ListService, LocalizationPipe, PermissionDirective } from '@abp/ng.core';
import {
  DateAdapter,
  NgxDatatableDefaultDirective,
  NgxDatatableListDirective,
} from '@abp/ng.theme.shared';
import { OrderItemViewService } from '../services/order-item-child.service';
import { OrderItemDetailViewService } from '../services/order-item-child-detail.service';
import { OrderItemDetailModalComponent } from './order-item-child-detail.component';
import { AbstractOrderItemComponent } from './order-item-child.abstract.component';

@Component({
  selector: 'app-order-item',
  imports: [
    NgbCollapseModule,
    NgbDatepickerModule,
    NgbDropdownModule,
    NgxValidateCoreModule,
    NgxDatatableModule,
    NgxDatatableDefaultDirective,
    NgxDatatableListDirective,
    PermissionDirective,
    LocalizationPipe,
    OrderItemDetailModalComponent,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    { provide: NgbDateAdapter, useClass: DateAdapter },
    ListService,
    OrderItemViewService,
    OrderItemDetailViewService,
  ],
  templateUrl: './order-item-child.component.html',
})
export class OrderItemComponent extends AbstractOrderItemComponent {}
