using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MiniExcelLibs;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.AuditLogging;

public class AuditLogExcelExportService : IAuditLogExcelExportService, ITransientDependency
{
    public async Task<Stream> ExportToExcelStreamAsync(List<AuditLog> auditLogs)
    {
        var exportData = MapToExcelRow(auditLogs);

        var stream = new MemoryStream();
        await stream.SaveAsAsync(exportData);
        
        stream.Position = 0;
        return stream;
    }
    
    public async Task CreateExcelFileAsync(List<AuditLog> auditLogs, Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }
        
        var exportData = MapToExcelRow(auditLogs);
        await stream.SaveAsAsync(exportData);
    }
    
    protected virtual object MapToExcelRow(List<AuditLog> auditLogs)
    {
        return auditLogs.Select(x => new
        {
            Id = x.Id,
            ExecutionTime = x.ExecutionTime.ToString("yyyy-MM-dd HH:mm:ss"),
            ExecutionDuration = x.ExecutionDuration,
            UserName = x.UserName,
            TenantName = x.TenantName,
            ClientIpAddress = x.ClientIpAddress,
            ClientId = x.ClientId,
            ClientName = x.ClientName,
            BrowserInfo = x.BrowserInfo,
            HttpMethod = x.HttpMethod,
            HttpStatusCode = x.HttpStatusCode,
            Url = x.Url,
            ApplicationName = x.ApplicationName,
            CorrelationId = x.CorrelationId,
            HasException = !string.IsNullOrEmpty(x.Exceptions),
            Comments = x.Comments,
            ImpersonatorUserName = x.ImpersonatorUserName,
            ImpersonatorTenantName = x.ImpersonatorTenantName
        }).ToList();
    }
} 