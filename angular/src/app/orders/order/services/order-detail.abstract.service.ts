import { inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ListService, TrackByService } from '@abp/ng.core';

import { finalize, tap } from 'rxjs/operators';

import { orderStatusOptions } from '../../../proxy/enums/orders/order-status.enum';
import type { OrderWithNavigationPropertiesDto } from '../../../proxy/orders/models';
import { OrderService } from '../../../proxy/orders/order.service';

export abstract class AbstractOrderDetailViewService {
  protected readonly fb = inject(FormBuilder);
  protected readonly track = inject(TrackByService);

  public readonly proxyService = inject(OrderService);
  public readonly list = inject(ListService);

  public readonly getIdentityUserLookup = this.proxyService.getIdentityUserLookup;

  orderStatusOptions = orderStatusOptions;

  isBusy = false;
  isVisible = false;
  selected = {} as any;
  form: FormGroup | undefined;

  protected createRequest() {
    const formValues = {
      ...this.form.value,
    };

    if (this.selected) {
      return this.proxyService.update(this.selected.order.id, {
        ...formValues,
        concurrencyStamp: this.selected.order.concurrencyStamp,
      });
    }

    return this.proxyService.create(formValues);
  }

  buildForm() {
    const { orderDate, totalAmount, status, identityUserId } = this.selected?.order || {};

    this.form = this.fb.group({
      orderDate: [orderDate ?? null, [Validators.required]],
      totalAmount: [totalAmount ?? '0', [Validators.required]],
      status: [status ?? null, [Validators.required]],
      identityUserId: [identityUserId ?? null, []],
    });
  }

  showForm() {
    this.buildForm();
    this.isVisible = true;
  }

  create() {
    this.selected = undefined;
    this.showForm();
  }

  update(record: OrderWithNavigationPropertiesDto) {
    this.selected = record;
    this.showForm();
  }

  hideForm() {
    this.isVisible = false;
  }

  submitForm() {
    if (this.form.invalid) return;

    this.isBusy = true;

    const request = this.createRequest().pipe(
      finalize(() => (this.isBusy = false)),
      tap(() => this.hideForm()),
    );

    request.subscribe(this.list.get);
  }

  changeVisible($event: boolean) {
    this.isVisible = $event;
  }
}
