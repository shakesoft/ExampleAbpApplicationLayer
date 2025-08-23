import type {
  GetOrderItemListInput,
  GetOrderItemsInput,
  OrderItemCreateDto,
  OrderItemDto,
  OrderItemUpdateDto,
  OrderItemWithNavigationPropertiesDto,
} from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type {
  AppFileDescriptorDto,
  DownloadTokenResultDto,
  GetFileInput,
  LookupDto,
  LookupRequestDto,
} from '../shared/models';

@Injectable({
  providedIn: 'root',
})
export class OrderItemService {
  apiName = 'Default';

  create = (input: OrderItemCreateDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrderItemDto>(
      {
        method: 'POST',
        url: '/api/app/order-items',
        body: input,
      },
      { apiName: this.apiName, ...config },
    );

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/app/order-items/${id}`,
      },
      { apiName: this.apiName, ...config },
    );

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrderItemDto>(
      {
        method: 'GET',
        url: `/api/app/order-items/${id}`,
      },
      { apiName: this.apiName, ...config },
    );

  getDownloadToken = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, DownloadTokenResultDto>(
      {
        method: 'GET',
        url: '/api/app/order-items/download-token',
      },
      { apiName: this.apiName, ...config },
    );

  getFile = (input: GetFileInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>(
      {
        method: 'GET',
        responseType: 'blob',
        url: '/api/app/order-items/file',
        params: { downloadToken: input.downloadToken, fileId: input.fileId },
      },
      { apiName: this.apiName, ...config },
    );

  getList = (input: GetOrderItemsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<OrderItemWithNavigationPropertiesDto>>(
      {
        method: 'GET',
        url: '/api/app/order-items',
        params: {
          orderId: input.orderId,
          filterText: input.filterText,
          sorting: input.sorting,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
          qtyMin: input.qtyMin,
          qtyMax: input.qtyMax,
          priceMin: input.priceMin,
          priceMax: input.priceMax,
          totalPriceMin: input.totalPriceMin,
          totalPriceMax: input.totalPriceMax,
          productName: input.productName,
          productId: input.productId,
        },
      },
      { apiName: this.apiName, ...config },
    );

  getListByOrderId = (input: GetOrderItemListInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<OrderItemWithNavigationPropertiesDto>>(
      {
        method: 'GET',
        url: '/api/app/order-items/by-order',
        params: {
          orderId: input.orderId,
          filterText: input.filterText,
          sorting: input.sorting,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
        },
      },
      { apiName: this.apiName, ...config },
    );

  getListWithNavigationPropertiesByOrderId = (
    input: GetOrderItemListInput,
    config?: Partial<Rest.Config>,
  ) =>
    this.restService.request<any, PagedResultDto<OrderItemWithNavigationPropertiesDto>>(
      {
        method: 'GET',
        url: '/api/app/order-items/detailed/by-order',
        params: {
          orderId: input.orderId,
          filterText: input.filterText,
          sorting: input.sorting,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
        },
      },
      { apiName: this.apiName, ...config },
    );

  getProductLookup = (input: LookupRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<LookupDto<string>>>(
      {
        method: 'GET',
        url: '/api/app/order-items/product-lookup',
        params: {
          filter: input.filter,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
        },
      },
      { apiName: this.apiName, ...config },
    );

  getWithNavigationProperties = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrderItemWithNavigationPropertiesDto>(
      {
        method: 'GET',
        url: `/api/app/order-items/with-navigation-properties/${id}`,
      },
      { apiName: this.apiName, ...config },
    );

  update = (id: string, input: OrderItemUpdateDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrderItemDto>(
      {
        method: 'PUT',
        url: `/api/app/order-items/${id}`,
        body: input,
      },
      { apiName: this.apiName, ...config },
    );

  uploadFile = (input: FormData, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AppFileDescriptorDto>(
      {
        method: 'POST',
        url: '/api/app/order-items/upload-file',
        body: input,
      },
      { apiName: this.apiName, ...config },
    );

  constructor(private restService: RestService) {}
}
