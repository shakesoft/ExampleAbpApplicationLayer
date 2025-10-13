namespace Volo.Abp.AuditLogging;

/// <summary>
/// Constants for email templates
/// </summary>
public static class AuditLoggingEmailTemplates
{
    /// <summary>
    /// Audit log export completed notification
    /// </summary>
    public const string AuditLogExportCompleted = "AuditLogExportCompleted";
    
    /// <summary>
    /// Audit log export failed notification
    /// </summary>
    public const string AuditLogExportFailed = "AuditLogExportFailed";
    
    /// <summary>
    /// Entity change export completed notification
    /// </summary>
    public const string EntityChangeExportCompleted = "EntityChangeExportCompleted";
    
    /// <summary>
    /// Entity change export failed notification
    /// </summary>
    public const string EntityChangeExportFailed = "EntityChangeExportFailed";
} 