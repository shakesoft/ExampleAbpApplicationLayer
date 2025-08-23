import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { ProductDto } from '../products/models';

export interface GetOrderItemListInput extends PagedAndSortedResultRequestDto {
  filterText?: string;
  orderId: string;
}

export interface GetOrderItemsInput extends PagedAndSortedResultRequestDto {
  filterText?: string;
  qtyMin?: number;
  qtyMax?: number;
  priceMin?: number;
  priceMax?: number;
  totalPriceMin?: number;
  totalPriceMax?: number;
  productName?: string;
  productId?: string;
  orderId: string;
}

export interface OrderItemCreateDto {
  qty: number;
  price: number;
  totalPrice: number;
  productName?: string;
  productId: string;
}

export interface OrderItemDto extends FullAuditedEntityDto<string> {
  qty: number;
  price: number;
  totalPrice: number;
  productName?: string;
  productId: string;
}

export interface OrderItemUpdateDto {
  qty: number;
  price: number;
  totalPrice: number;
  productName?: string;
  productId: string;
}

export interface OrderItemWithNavigationPropertiesDto {
  orderItem: OrderItemDto;
  product: ProductDto;
}
