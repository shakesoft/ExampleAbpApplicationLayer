import type {
  GetProductsInput,
  ProductCreateDto,
  ProductDto,
  ProductExcelDownloadDto,
  ProductUpdateDto,
} from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { AppFileDescriptorDto, DownloadTokenResultDto, GetFileInput } from '../shared/models';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  apiName = 'Default';

  create = (input: ProductCreateDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>(
      {
        method: 'POST',
        url: '/api/app/products',
        body: input,
      },
      { apiName: this.apiName, ...config },
    );

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: `/api/app/products/${id}`,
      },
      { apiName: this.apiName, ...config },
    );

  deleteAll = (input: GetProductsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: '/api/app/products/all',
        params: {
          filterText: input.filterText,
          sorting: input.sorting,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
          name: input.name,
          desc: input.desc,
          priceMin: input.priceMin,
          priceMax: input.priceMax,
          isActive: input.isActive,
        },
      },
      { apiName: this.apiName, ...config },
    );

  deleteByIds = (productIds: string[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>(
      {
        method: 'DELETE',
        url: '/api/app/products',
        params: { productIds },
      },
      { apiName: this.apiName, ...config },
    );

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>(
      {
        method: 'GET',
        url: `/api/app/products/${id}`,
      },
      { apiName: this.apiName, ...config },
    );

  getDownloadToken = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, DownloadTokenResultDto>(
      {
        method: 'GET',
        url: '/api/app/products/download-token',
      },
      { apiName: this.apiName, ...config },
    );

  getFile = (input: GetFileInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>(
      {
        method: 'GET',
        responseType: 'blob',
        url: '/api/app/products/file',
        params: { downloadToken: input.downloadToken, fileId: input.fileId },
      },
      { apiName: this.apiName, ...config },
    );

  getList = (input: GetProductsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProductDto>>(
      {
        method: 'GET',
        url: '/api/app/products',
        params: {
          filterText: input.filterText,
          sorting: input.sorting,
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
          name: input.name,
          desc: input.desc,
          priceMin: input.priceMin,
          priceMax: input.priceMax,
          isActive: input.isActive,
        },
      },
      { apiName: this.apiName, ...config },
    );

  getListAsExcelFile = (input: ProductExcelDownloadDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>(
      {
        method: 'GET',
        responseType: 'blob',
        url: '/api/app/products/as-excel-file',
        params: {
          downloadToken: input.downloadToken,
          filterText: input.filterText,
          name: input.name,
          desc: input.desc,
          priceMin: input.priceMin,
          priceMax: input.priceMax,
          isActive: input.isActive,
        },
      },
      { apiName: this.apiName, ...config },
    );

  update = (id: string, input: ProductUpdateDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>(
      {
        method: 'PUT',
        url: `/api/app/products/${id}`,
        body: input,
      },
      { apiName: this.apiName, ...config },
    );

  uploadFile = (input: FormData, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AppFileDescriptorDto>(
      {
        method: 'POST',
        url: '/api/app/products/upload-file',
        body: input,
      },
      { apiName: this.apiName, ...config },
    );

  constructor(private restService: RestService) {}
}
