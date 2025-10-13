using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.AuditLogging.ExcelFileDownload;
using Volo.Abp.Auditing;
using Xunit;

namespace Volo.Abp.AuditLogging;

public class AuditLogExcelExport_Tests : AbpAuditLoggingTestBase<AbpAuditLoggingTestBaseModule>
{
    private readonly IAuditLogsAppService _auditLogsAppService;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IAuditLogInfoToAuditLogConverter _auditLogInfoToAuditLogConverter;
    private readonly IAuditLogExcelExportService _auditLogExcelExportService;
    private readonly IEntityChangeExcelExportService _entityChangeExcelExportService;
    private readonly IExcelFileDownloadService _excelFileDownloadService;

    public AuditLogExcelExport_Tests()
    {
        _auditLogsAppService = GetRequiredService<IAuditLogsAppService>();
        _auditLogRepository = GetRequiredService<IAuditLogRepository>();
        _auditLogInfoToAuditLogConverter = GetRequiredService<IAuditLogInfoToAuditLogConverter>();
        _auditLogExcelExportService = GetRequiredService<IAuditLogExcelExportService>();
        _entityChangeExcelExportService = GetRequiredService<IEntityChangeExcelExportService>();
        _excelFileDownloadService = GetRequiredService<IExcelFileDownloadService>();
    }

    [Fact]
    public async Task Should_Export_AuditLogs_To_Excel()
    {
        var log = await CreateAuditLogAsync();
        await _auditLogRepository.InsertAsync(log);

        // Act
        var result = await _auditLogsAppService.ExportToExcelAsync(new ExportAuditLogsInput
        {
            StartTime = DateTime.Today.AddDays(-1),
            EndTime = DateTime.Today.AddDays(1)
        });

        // Assert
        result.ShouldNotBeNull();
        result.IsBackgroundJob.ShouldBeFalse();
        result.FileData.ShouldNotBeNull();
        result.FileName.ShouldNotBeNullOrEmpty();
        result.FileName.ShouldEndWith(".xlsx");
    }

    [Fact]
    public async Task Should_Export_EntityChanges_To_Excel()
    {
        // Arrange
        var log = await CreateAuditLogAsync();

        await _auditLogRepository.InsertAsync(log);

        // Act
        var result = await _auditLogsAppService.ExportEntityChangesToExcelAsync(new ExportEntityChangesInput
        {
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(1)
        });

        // Assert
        result.ShouldNotBeNull();
        result.IsBackgroundJob.ShouldBeFalse();
        result.FileData.ShouldNotBeNull();
        result.FileName.ShouldNotBeNullOrEmpty();
        result.FileName.ShouldEndWith(".xlsx");
    }

    [Fact]
    public async Task AuditLogExcelExportService_Should_Export_To_Stream()
    {
        // Arrange
        var auditLog = await CreateAuditLogAsync();

        // Act
        var stream = await _auditLogExcelExportService.ExportToExcelStreamAsync(new List<AuditLog> { auditLog });

        // Assert
        stream.ShouldNotBeNull();
        stream.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task EntityChangeExcelExportService_Should_Export_To_Stream()
    {
        // Arrange
        var auditLog = await CreateAuditLogAsync();

        // Act
        var stream = await _entityChangeExcelExportService.ExportToExcelStreamAsync(auditLog.EntityChanges.ToList());

        // Assert
        stream.ShouldNotBeNull();
        stream.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void ExcelFileDownloadService_Should_Generate_Unique_FileName()
    {
        // Act
        var fileName1 = _excelFileDownloadService.GenerateUniqueFileName("TestFile", ".xlsx");
        var fileName2 = _excelFileDownloadService.GenerateUniqueFileName("TestFile", ".xlsx");

        // Assert
        fileName1.ShouldNotBeNullOrEmpty();
        fileName2.ShouldNotBeNullOrEmpty();
        fileName1.ShouldStartWith("TestFile_");
        fileName2.ShouldStartWith("TestFile_");
        fileName1.ShouldNotBe(fileName2); // Should be unique
    }

    [Fact]
    public async Task ExcelFileDownloadService_Should_Create_Download_Link()
    {
        // Arrange
        var fileName = "test_file.xlsx";

        // Act
        var downloadLink = await _excelFileDownloadService.CreateDownloadLinkAsync(fileName);

        // Assert
        downloadLink.ShouldNotBeNullOrEmpty();
        downloadLink.ShouldContain(fileName);
        downloadLink.ShouldContain("/api/audit-logging/audit-logs/download-excel/");
    }


    private async Task<AuditLog> CreateAuditLogAsync()
    {
        var userId = new Guid("4456fb0d-74cc-4807-9eee-23e551e6cb06");
        var ipAddress = "153.1.7.61";

        var log = new AuditLogInfo
        {
            UserId = userId,
            ExecutionTime = DateTime.Today,
            ExecutionDuration = 42,
            ClientIpAddress = ipAddress,
            EntityChanges = {
                new EntityChangeInfo
                {
                    EntityId = Guid.NewGuid().ToString(),
                    EntityTypeFullName = "Volo.Abp.AuditLogging.TestEntity",
                    ChangeType = EntityChangeType.Created,
                    ChangeTime = DateTime.Now,
                    PropertyChanges = new List<EntityPropertyChangeInfo>
                    {
                        new EntityPropertyChangeInfo
                        {
                            PropertyTypeFullName = typeof(string).FullName,
                            PropertyName = "Name",
                            NewValue = "New value",
                            OriginalValue = null
                        }
                    }
                }
            }
        };

        return await _auditLogInfoToAuditLogConverter.ConvertAsync(log);
    }
} 