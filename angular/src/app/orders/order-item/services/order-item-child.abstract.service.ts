import { inject, ChangeDetectorRef } from '@angular/core';
import { filter, switchMap } from 'rxjs/operators';
import { ABP, ListService, PagedResultDto } from '@abp/ng.core';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import type {
  GetOrderItemListInput,
  OrderItemWithNavigationPropertiesDto,
} from '../../../proxy/order-items/models';
import { OrderItemService } from '../../../proxy/order-items/order-item.service';

export abstract class AbstractOrderItemViewService {
  protected readonly cdr = inject(ChangeDetectorRef);
  protected readonly proxyService = inject(OrderItemService);
  protected readonly confirmationService = inject(ConfirmationService);
  protected readonly list = inject(ListService);

  data: PagedResultDto<OrderItemWithNavigationPropertiesDto> = {
    items: [],
    totalCount: 0,
  };

  delete(record: OrderItemWithNavigationPropertiesDto) {
    this.confirmationService
      .warn('::DeleteConfirmationMessage', '::AreYouSure', { messageLocalizationParams: [] })
      .pipe(
        filter(status => status === Confirmation.Status.confirm),
        switchMap(() => this.proxyService.delete(record.orderItem.id)),
      )
      .subscribe(this.list.get);
  }

  hookToQuery(orderId: string) {
    const getData = (query: ABP.PageQueryParams) =>
      this.proxyService.getListWithNavigationPropertiesByOrderId({
        ...(query as GetOrderItemListInput),
        orderId,
      });

    const setData = (list: PagedResultDto<OrderItemWithNavigationPropertiesDto>) =>
      (this.data = list);

    this.list.hookToQuery(getData).subscribe(list => {
      setData(list);
      this.cdr.markForCheck();
    });
  }
}
