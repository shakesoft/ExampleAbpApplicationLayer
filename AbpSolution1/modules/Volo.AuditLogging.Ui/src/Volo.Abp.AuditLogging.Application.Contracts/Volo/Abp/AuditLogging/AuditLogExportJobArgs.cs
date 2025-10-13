using System;

namespace Volo.Abp.AuditLogging;

[Serializable]
public class AuditLogExportJobArgs
{
    public Guid? UserId { get; set; }
    public Guid? TenantId { get; set; }
    public string Email { get; set; }
    public ExportAuditLogsInput Filter { get; set; }
    public string Culture { get; set; }
}