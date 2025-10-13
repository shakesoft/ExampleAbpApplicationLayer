namespace Volo.Abp.AuditLogging.ExcelFileDownload;

/// <summary>
/// Options for file cleanup worker
/// </summary>
public class ExcelFileCleanupOptions
{
    /// <summary>
    /// The period to run the cleanup worker (in milliseconds)
    /// </summary>
    public int Period { get; set; } = 86400000; // Default: 24 hours

    /// <summary>
    /// Default: Every day at 23:00
    /// This Cron expression only works if Hangfire or Quartz is used for background workers.
    /// </summary>
    public string CronExpression { get; set; } = "0 23 * * *";
}
