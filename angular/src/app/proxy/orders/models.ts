import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { OrderStatus } from '../enums/orders/order-status.enum';

export interface GetOrdersInput extends PagedAndSortedResultRequestDto {
  filterText?: string;
  orderDateMin?: string;
  orderDateMax?: string;
  totalAmountMin?: number;
  totalAmountMax?: number;
  status?: OrderStatus;
}

export interface OrderCreateDto {
  orderDate?: string;
  totalAmount: number;
  status: OrderStatus;
}

export interface OrderDto extends FullAuditedEntityDto<string> {
  orderDate?: string;
  totalAmount: number;
  status: OrderStatus;
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
}

export interface OrderUpdateDto {
  orderDate?: string;
  totalAmount: number;
  status: OrderStatus;
  concurrencyStamp?: string;
}
