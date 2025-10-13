namespace Volo.Abp.AuditLogging;

/// <summary>
/// Audit logs export result
/// </summary>
public class ExportAuditLogsOutput
{
    /// <summary>
    /// Message to display to the user
    /// </summary>
    public string Message { get; set; }
    
    /// <summary>
    /// File data (for direct download)
    /// </summary>
    public byte[] FileData { get; set; }
    
    /// <summary>
    /// File name
    /// </summary>
    public string FileName { get; set; }
    
    /// <summary>
    /// Is running as a background job
    /// </summary>
    public bool IsBackgroundJob { get; set; }
} 