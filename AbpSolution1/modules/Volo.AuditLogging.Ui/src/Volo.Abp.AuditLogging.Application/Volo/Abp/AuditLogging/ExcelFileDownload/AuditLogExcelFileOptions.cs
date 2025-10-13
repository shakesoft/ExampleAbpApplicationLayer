using System;

namespace Volo.Abp.AuditLogging.ExcelFileDownload;

public class AuditLogExcelFileOptions
{
    /// <summary>
    /// Time period (in hours) for which files will be kept before automatic deletion. Default value: 24 hours (1 day)
    /// </summary>
    public int FileRetentionHours { get; set; } = 24;

    /// <summary>
    /// Base URL for generating file download links
    /// </summary>
    public string DownloadBaseUrl { get; set; }

    public ExcelFileCleanupOptions ExcelFileCleanupOptions { get; set; } = new ExcelFileCleanupOptions
    {
        Period = (int)TimeSpan.FromHours(24).TotalMilliseconds // Default: 24 hours
    };
}