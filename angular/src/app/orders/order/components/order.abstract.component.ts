import { Directive, OnInit, inject, ViewChild } from '@angular/core';

import {
  NgbNav,
  NgbNavItem,
  NgbNavLink,
  NgbNavContent,
  NgbNavOutlet,
} from '@ng-bootstrap/ng-bootstrap';
import { ListService, PermissionService, TrackByService } from '@abp/ng.core';

import { orderStatusOptions } from '../../../proxy/enums/orders/order-status.enum';
import type { OrderDto } from '../../../proxy/orders/models';
import { OrderViewService } from '../services/order.service';
import { OrderDetailViewService } from '../services/order-detail.service';
import { OrderItemComponent } from '../../order-item/components/order-item-child.component';

export const ChildTabDependencies = [NgbNav, NgbNavItem, NgbNavLink, NgbNavContent, NgbNavOutlet];

export const ChildComponentDependencies = [OrderItemComponent];

@Directive()
export abstract class AbstractOrderComponent implements OnInit {
  public readonly list = inject(ListService);
  public readonly track = inject(TrackByService);
  public readonly service = inject(OrderViewService);
  public readonly serviceDetail = inject(OrderDetailViewService);
  public readonly permissionService = inject(PermissionService);

  protected title = '::Orders';
  protected isActionButtonVisible: boolean | null = null;
  protected isChildEntitiesPermitted: boolean | null = null;

  orderStatusOptions = orderStatusOptions;

  @ViewChild('orderTable') table: any;

  ngOnInit() {
    this.service.hookToQuery();
    this.checkActionButtonVisibility();
    this.checkChildEntityPermissions();
  }

  clearFilters() {
    this.service.clearFilters();
  }

  showForm() {
    this.serviceDetail.showForm();
  }

  create() {
    this.serviceDetail.selected = undefined;
    this.serviceDetail.showForm();
  }

  update(record: OrderDto) {
    this.serviceDetail.update(record);
  }

  delete(record: OrderDto) {
    this.service.delete(record);
  }

  exportToExcel() {
    this.service.exportToExcel();
  }

  toggleExpandRow(row) {
    this.table.rowDetail.toggleExpandRow(row);
  }

  checkChildEntityPermissions() {
    if (this.isChildEntitiesPermitted !== null) {
      return;
    }

    const childPermissions = ['ExampleAbpApplicationLayer.OrderItems'];
    this.isChildEntitiesPermitted = childPermissions.some(permission =>
      this.permissionService.getGrantedPolicy(permission),
    );
  }

  checkActionButtonVisibility() {
    if (this.isActionButtonVisible !== null) {
      return;
    }

    const canEdit = this.permissionService.getGrantedPolicy(
      'ExampleAbpApplicationLayer.Orders.Edit',
    );
    const canDelete = this.permissionService.getGrantedPolicy(
      'ExampleAbpApplicationLayer.Orders.Delete',
    );
    this.isActionButtonVisible = canEdit || canDelete;
  }
}
