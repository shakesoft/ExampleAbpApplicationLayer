import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface GetProductsInput extends PagedAndSortedResultRequestDto {
  filterText?: string;
  name?: string;
  desc?: string;
  priceMin?: number;
  priceMax?: number;
  isActive?: boolean;
}

export interface ProductCreateDto {
  name: string;
  desc?: string;
  price: number;
  isActive: boolean;
}

export interface ProductDto extends FullAuditedEntityDto<string> {
  name: string;
  desc?: string;
  price: number;
  isActive: boolean;
  concurrencyStamp?: string;
}

export interface ProductExcelDownloadDto {
  downloadToken?: string;
  filterText?: string;
  name?: string;
  desc?: string;
  priceMin?: number;
  priceMax?: number;
  isActive?: boolean;
}

export interface ProductUpdateDto {
  name: string;
  desc?: string;
  price: number;
  isActive: boolean;
  concurrencyStamp?: string;
}
