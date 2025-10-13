using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Volo.Abp.AuditLogging;

public interface IAuditLogExcelExportService
{
    Task<Stream> ExportToExcelStreamAsync(List<AuditLog> auditLogs);
    
    Task CreateExcelFileAsync(List<AuditLog> auditLogs, Stream stream);
} 