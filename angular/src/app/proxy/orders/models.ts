import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { OrderStatus } from '../enums/orders/order-status.enum';
import type { IdentityUserDto } from '../volo/abp/identity/models';

export interface GetOrdersInput extends PagedAndSortedResultRequestDto {
  filterText?: string;
  orderDateMin?: string;
  orderDateMax?: string;
  totalAmountMin?: number;
  totalAmountMax?: number;
  status?: OrderStatus;
  identityUserId?: string;
}

export interface OrderCreateDto {
  orderDate?: string;
  totalAmount: number;
  status: OrderStatus;
  identityUserId?: string;
}

export interface OrderDto extends FullAuditedEntityDto<string> {
  orderDate?: string;
  totalAmount: number;
  status: OrderStatus;
  identityUserId?: string;
  concurrencyStamp?: string;
}

export interface OrderExcelDownloadDto {
  downloadToken?: string;
  filterText?: string;
  orderDateMin?: string;
  orderDateMax?: string;
  totalAmountMin?: number;
  totalAmountMax?: number;
  status?: OrderStatus;
  identityUserId?: string;
}

export interface OrderUpdateDto {
  orderDate?: string;
  totalAmount: number;
  status: OrderStatus;
  identityUserId?: string;
  concurrencyStamp?: string;
}

export interface OrderWithNavigationPropertiesDto {
  order: OrderDto;
  identityUser: IdentityUserDto;
}
