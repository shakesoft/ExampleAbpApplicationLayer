import { Component, inject, Input, Output, ViewChild } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ChartComponent } from '@abp/ng.components/chart.js';
import {
  LocalizationPipe,
  LocalizationService,
  PermissionDirective,
  PermissionService,
} from '@abp/ng.core';
import { Statistics } from '@abp/ng.theme.shared';
import { AuditLogsService } from '@volo/abp.ng.audit-logging/proxy';

@Component({
  selector: 'abp-average-execution-duration-widget',
  template: `
    <div *abpPermission="'AuditLogging.AuditLogs'" class="abp-widget-wrapper">
      <div class="card">
        <div class="card-header">
          <h5 class="card-title">
            {{ 'AbpAuditLogging::AverageExecutionDurationInLogsPerDay' | abpLocalization }}
          </h5>
        </div>
        <div class="card-body">
          <div class="row">
            <abp-chart
              #chart
              class="w-100"
              type="bar"
              [data]="chartData"
              [width]="width"
              [height]="height"
            />
          </div>
        </div>
      </div>
    </div>
  `,
  imports: [ChartComponent, PermissionDirective, LocalizationPipe],
})
export class AverageExecutionDurationWidgetComponent {
  protected localizationService = inject(LocalizationService);
  protected permissionService = inject(PermissionService);
  protected service = inject(AuditLogsService);

  data: Statistics.Data = {};

  @Output() initialized = new BehaviorSubject(this);

  @Input() width = 273;

  @Input() height = 136;

  @ViewChild(ChartComponent) chart: ChartComponent;

  chartData: any = {};

  draw = (filter: { startDate: string; endDate: string }) => {
    if (!this.permissionService.getGrantedPolicy('AuditLogging.AuditLogs')) {
      return;
    }

    this.service
      .getAverageExecutionDurationPerDay({ startDate: filter.startDate, endDate: filter.endDate })
      .subscribe(res => {
        this.data = res.data;
        this.setChartData();
      });
  };

  private setChartData() {
    if (!this.data || JSON.stringify(this.data) === '{}') {
      this.chartData = {};
      return;
    }
    const dataKeys = Object.keys(this.data);

    setTimeout(() => {
      this.chartData = {
        labels: dataKeys,
        datasets: [
          {
            label: this.localizationService.instant(
              'AbpAuditLogging::AverageExecutionDurationInMilliseconds',
            ),
            backgroundColor: '#fa6e6e',
            data: dataKeys.map(key => this.data[key]),
          },
        ],
      };
      this.chart.refresh();
    }, 0);
  }
}
