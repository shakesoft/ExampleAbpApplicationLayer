import { DestroyRef, Injector } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable, of, switchMap, tap } from 'rxjs';
import { AbpWindowService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { ExportAuditLogsOutput, ExportEntityChangesOutput } from '@volo/abp.ng.audit-logging/proxy';

export function handleExcelExport(
  exportObservable: Observable<ExportAuditLogsOutput | ExportEntityChangesOutput>,
  injector: Injector,
): void {
  const abpWindowService = injector.get(AbpWindowService);
  const toasterService = injector.get(ToasterService);
  const destroyRef = injector.get(DestroyRef);

  if (!abpWindowService || !toasterService) {
    toasterService.error('AbpAuditLogging::ExportFailed');
    return;
  }

  exportObservable
    .pipe(
      switchMap((file: ExportAuditLogsOutput | ExportEntityChangesOutput) => {
        const binaryString = atob(String(file.fileData));
        const byteArray = Uint8Array.from(binaryString, char => char.charCodeAt(0));

        const blob = new Blob([byteArray], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        });

        return of({ blob, fileName: file.fileName });
      }),
      tap(({ blob, fileName }) => {
        try {
          abpWindowService.downloadBlob(blob, fileName);
          toasterService.success('AbpAuditLogging::ExportCompleted');
        } catch (error) {
          toasterService.error('AbpAuditLogging::ExportFailed');
          console.error('Download failed:', error);
        }
      }),
      takeUntilDestroyed(destroyRef),
    )
    .subscribe();
}
