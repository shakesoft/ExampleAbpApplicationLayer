import { inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ListService, TrackByService } from '@abp/ng.core';

import { finalize, tap } from 'rxjs/operators';

import type { OrderItemWithNavigationPropertiesDto } from '../../../proxy/order-items/models';
import { OrderItemService } from '../../../proxy/order-items/order-item.service';

export abstract class AbstractOrderItemDetailViewService {
  protected readonly fb = inject(FormBuilder);
  protected readonly track = inject(TrackByService);

  public readonly proxyService = inject(OrderItemService);
  public readonly list = inject(ListService);

  public readonly getProductLookup = this.proxyService.getProductLookup;

  orderId: string;

  isBusy = false;
  isVisible = false;
  selected = {} as any;
  form: FormGroup | undefined;

  protected createRequest() {
    if (this.selected) {
      return this.proxyService.update(this.selected.id, this.form.value);
    }
    return this.proxyService.create(this.form.value);
  }

  buildForm() {
    const { productId, qty, price, totalPrice, productName } = this.selected || {};

    this.form = this.fb.group({
      orderId: [this.orderId],
      productId: [productId ?? null, [Validators.required]],
      qty: [qty ?? '1', [Validators.required]],
      price: [price ?? '0', [Validators.required]],
      totalPrice: [totalPrice ?? null, [Validators.required]],
      productName: [productName ?? null, []],
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

  update(record: OrderItemWithNavigationPropertiesDto) {
    this.selected = record.orderItem;
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

  changeVisible(isVisible: boolean) {
    this.isVisible = isVisible;
  }
}
