import { Directive, OnInit, inject } from '@angular/core';

import { ListService, PermissionService, TrackByService } from '@abp/ng.core';

import type { ProductDto } from '../../../proxy/products/models';
import { ProductViewService } from '../services/product.service';
import { ProductDetailViewService } from '../services/product-detail.service';

export const ChildTabDependencies = [];

export const ChildComponentDependencies = [];

@Directive()
export abstract class AbstractProductComponent implements OnInit {
  public readonly list = inject(ListService);
  public readonly track = inject(TrackByService);
  public readonly service = inject(ProductViewService);
  public readonly serviceDetail = inject(ProductDetailViewService);
  public readonly permissionService = inject(PermissionService);

  protected title = '::Products';
  protected isActionButtonVisible: boolean | null = null;

  ngOnInit() {
    this.service.hookToQuery();
    this.checkActionButtonVisibility();
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

  update(record: ProductDto) {
    this.serviceDetail.update(record);
  }

  delete(record: ProductDto) {
    this.service.delete(record);
  }

  exportToExcel() {
    this.service.exportToExcel();
  }

  checkActionButtonVisibility() {
    if (this.isActionButtonVisible !== null) {
      return;
    }

    const canEdit = this.permissionService.getGrantedPolicy(
      'ExampleAbpApplicationLayer.Products.Edit',
    );
    const canDelete = this.permissionService.getGrantedPolicy(
      'ExampleAbpApplicationLayer.Products.Delete',
    );
    this.isActionButtonVisible = canEdit || canDelete;
  }
}
