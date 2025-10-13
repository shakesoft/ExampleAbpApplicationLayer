import { mapEnumToOptions } from '@abp/ng.core';

export enum eIdleTimeoutMinutes {
  OneHour = 60,
  ThreeHours = 180,
  SixHours = 360,
  TwelveHours = 720,
  TwentyFourHours = 1440,
  CustomIdleTimeoutMinutes = 1,
}

export const idleTimeoutMinuteOptions = mapEnumToOptions(eIdleTimeoutMinutes);
