import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { ModuleWithProviders, NgModule } from '@angular/core';
import { EntityChangeModalComponent } from './components/entity-change-modal.component';
import { provideAuditLoggingConfig } from './providers';
import { EntityChangeDetailsComponent } from '@volo/abp.ng.audit-logging/common';

@NgModule({
  exports: [EntityChangeModalComponent],
  imports: [
    CoreModule,
    ThemeSharedModule,
    EntityChangeDetailsComponent,
    EntityChangeModalComponent,
  ],
})
export class AuditLoggingConfigModule {
  /**
   * @deprecated forRoot method is deprecated, use `provideAuditLoggingConfig` *function* for config settings.
   */
  static forRoot(): ModuleWithProviders<AuditLoggingConfigModule> {
    return {
      ngModule: AuditLoggingConfigModule,
      providers: [provideAuditLoggingConfig()],
    };
  }
}
