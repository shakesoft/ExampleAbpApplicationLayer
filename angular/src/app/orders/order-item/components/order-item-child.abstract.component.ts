import { Directive, OnInit, inject, Input } from '@angular/core';
import { ListService, PermissionService, TrackByService } from '@abp/ng.core';

import type { OrderItemWithNavigationPropertiesDto } from '../../../proxy/order-items/models';
import { OrderItemViewService } from '../services/order-item-child.service';
import { OrderItemDetailViewService } from '../services/order-item-child-detail.service';

@Directive()
export abstract class AbstractOrderItemComponent implements OnInit {
  public readonly list = inject(ListService);
  public readonly track = inject(TrackByService);
  public readonly service = inject(OrderItemViewService);
  public readonly serviceDetail = inject(OrderItemDetailViewService);
  public readonly permissionService = inject(PermissionService);

  protected isActionButtonVisible: boolean | null = null;

  @Input() title = '::OrderItems';
  @Input() orderId: string;

  ngOnInit() {
    this.serviceDetail.orderId = this.orderId;
    this.service.hookToQuery(this.orderId);
    this.checkActionButtonVisibility();
  }

  create() {
    this.serviceDetail.selected = undefined;
    this.serviceDetail.showForm();
  }

  update(record: OrderItemWithNavigationPropertiesDto) {
    this.serviceDetail.update(record);
  }

  delete(record: OrderItemWithNavigationPropertiesDto) {
    this.service.delete(record);
  }

  checkActionButtonVisibility() {
    if (this.isActionButtonVisible !== null) {
      return;
    }

    const canEdit = this.permissionService.getGrantedPolicy(
      'ExampleAbpApplicationLayer.OrderItems.Edit',
    );
    const canDelete = this.permissionService.getGrantedPolicy(
      'ExampleAbpApplicationLayer.OrderItems.Delete',
    );
    this.isActionButtonVisible = canEdit || canDelete;
  }
}
