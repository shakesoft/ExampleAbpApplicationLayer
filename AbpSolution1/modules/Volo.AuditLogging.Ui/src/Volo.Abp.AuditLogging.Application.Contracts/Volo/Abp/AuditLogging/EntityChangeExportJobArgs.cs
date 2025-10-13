using System;

namespace Volo.Abp.AuditLogging;

[Serializable]
public class EntityChangeExportJobArgs
{
    public Guid? UserId { get; set; }
    public Guid? TenantId { get; set; }
    public string Email { get; set; }
    public string Culture { get; set; }
    public ExportEntityChangesInput Filter { get; set; }
} 