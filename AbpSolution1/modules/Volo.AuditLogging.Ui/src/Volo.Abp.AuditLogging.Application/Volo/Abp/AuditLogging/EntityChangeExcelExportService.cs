using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MiniExcelLibs;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.AuditLogging;

public class EntityChangeExcelExportService : IEntityChangeExcelExportService, ITransientDependency
{
    public async Task<Stream> ExportToExcelStreamAsync(List<EntityChange> entityChanges)
    {
        var exportData = MapToExcelRow(entityChanges);

        var stream = new MemoryStream();
        await stream.SaveAsAsync(exportData);
        
        stream.Position = 0;
        return stream;
    }
    
    public async Task CreateExcelFileAsync(List<EntityChange> entityChanges, Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }
        
        var exportData = MapToExcelRow(entityChanges);
        await stream.SaveAsAsync(exportData);
    }
    
    protected virtual object MapToExcelRow(List<EntityChange> entityChanges)
    {
        return entityChanges.Select(x => new
        {
            Id = x.Id,
            ChangeTime = x.ChangeTime.ToString("yyyy-MM-dd HH:mm:ss"),
            ChangeType = x.ChangeType.ToString(),
            EntityId = x.EntityId,
            EntityTypeFullName = x.EntityTypeFullName,
            TenantId = x.TenantId?.ToString() ?? "null",
            AuditLogId = x.AuditLogId,
            PropertyChangesCount = x.PropertyChanges?.Count ?? 0
        }).ToList();
    }
} 