using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Volo.Abp.AuditLogging;

public interface IEntityChangeExcelExportService
{
    Task<Stream> ExportToExcelStreamAsync(List<EntityChange> entityChanges);
    
    Task CreateExcelFileAsync(List<EntityChange> entityChanges, Stream stream);
} 