import type {
  GetOrdersInput,
  OrderCreateDto,
  OrderDto,
  OrderExcelDownloadDto,
  OrderUpdateDto,
} from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { AppFileDescriptorDto, DownloadTokenResultDto, GetFileInput } from '../shared/models';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  apiName = 'Default';

  create = (input: OrderCreateDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrderDto>(
      {
        method: 'POST',
        url: '/api/app/orders',
        body: input,
      },
      { apiName: this.apiName, ...config },
    );

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/app/orders/${id}`,
      },
      { apiName: this.apiName, ...config },
    );

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrderDto>(
      {
        method: 'GET',
        url: `/api/app/orders/${id}`,
      },
      { apiName: this.apiName, ...config },
    );

  getDownloadToken = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, DownloadTokenResultDto>(
      {
        method: 'GET',
        url: '/api/app/orders/download-token',
      },
      { apiName: this.apiName, ...config },
    );

  getFile = (input: GetFileInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>(
      {
        method: 'GET',
        responseType: 'blob',
        url: '/api/app/orders/file',
        params: { downloadToken: input.downloadToken, fileId: input.fileId },
      },
      { apiName: this.apiName, ...config },
    );

  getList = (input: GetOrdersInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<OrderDto>>(
      {
        method: 'GET',
        url: '/api/app/orders',
        params: {
          filterText: input.filterText,
          sorting: input.sorting,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
          orderDateMin: input.orderDateMin,
          orderDateMax: input.orderDateMax,
          totalAmountMin: input.totalAmountMin,
          totalAmountMax: input.totalAmountMax,
          status: input.status,
        },
      },
      { apiName: this.apiName, ...config },
    );

  getListAsExcelFile = (input: OrderExcelDownloadDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>(
      {
        method: 'GET',
        responseType: 'blob',
        url: '/api/app/orders/as-excel-file',
        params: {
          downloadToken: input.downloadToken,
          filterText: input.filterText,
          orderDateMin: input.orderDateMin,
          orderDateMax: input.orderDateMax,
          totalAmountMin: input.totalAmountMin,
          totalAmountMax: input.totalAmountMax,
          status: input.status,
        },
      },
      { apiName: this.apiName, ...config },
    );

  update = (id: string, input: OrderUpdateDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrderDto>(
      {
        method: 'PUT',
        url: `/api/app/orders/${id}`,
        body: input,
      },
      { apiName: this.apiName, ...config },
    );

  uploadFile = (input: FormData, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AppFileDescriptorDto>(
      {
        method: 'POST',
        url: '/api/app/orders/upload-file',
        body: input,
      },
      { apiName: this.apiName, ...config },
    );

  constructor(private restService: RestService) {}
}
