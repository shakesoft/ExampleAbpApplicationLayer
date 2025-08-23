import { mapEnumToOptions } from '@abp/ng.core';

export enum OrderStatus {
  Initialized = 0,
  Paid = 1,
  Processing = 2,
  Ordered = 3,
  Shipped = 4,
  Arrived = 5,
  Delivered = 6,
  Cancelled = 7,
  NotPaid = 8,
}

export const orderStatusOptions = mapEnumToOptions(OrderStatus);
