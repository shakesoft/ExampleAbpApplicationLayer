using System;
using JetBrains.Annotations;

namespace Volo.Abp.AuditLogging;

public class ExpiredAuditLogDeleterOptions
{
    /// <summary>
    /// Default: Everyday once.
    /// </summary>
    public int Period { get; set; } = (int)TimeSpan.FromDays(1).TotalMilliseconds;

    /// <summary>
    /// Default: Every day at 23:00
    /// This Cron expression only works if Hangfire or Quartz is used for background workers.
    /// </summary>
    public string CronExpression { get; set; } = "0 23 * * *";
}
