using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Volo.Abp.AuditLogging.ExcelFileDownload;
using Volo.Abp.BlobStoring;
using Xunit;

namespace Volo.Abp.AuditLogging;

public class AuditLogExportJobs_Tests : AbpAuditLoggingTestBase<AbpAuditLoggingTestBaseModule>
{
    private readonly AuditLogExportJob _auditLogExportJob;
    private readonly EntityChangeExportJob _entityChangeExportJob;
    private readonly IExcelFileDownloadService _excelFileDownloadService;
    private readonly IBlobContainer<AuditLogExcelContainer> _blobContainer;

    public AuditLogExportJobs_Tests()
    {
        _auditLogExportJob = GetRequiredService<AuditLogExportJob>();
        _entityChangeExportJob = GetRequiredService<EntityChangeExportJob>();
        _excelFileDownloadService = GetRequiredService<IExcelFileDownloadService>();
        _blobContainer = GetRequiredService<IBlobContainer<AuditLogExcelContainer>>();
    }

    [Fact]
    public void AuditLogExportJob_Should_Be_Registered()
    {
        // Assert
        _auditLogExportJob.ShouldNotBeNull();
    }

    [Fact]
    public void EntityChangeExportJob_Should_Be_Registered()
    {
        // Assert
        _entityChangeExportJob.ShouldNotBeNull();
    }

    [Fact]
    public async Task ExcelFileCleanupWorker_Should_Be_Registered()
    {
        // Arrange
        var worker = GetService<ExcelFileCleanupWorker>();

        // Assert
        worker.ShouldNotBeNull();
    }

    [Fact]
    public async Task AuditLogExportJob_Should_Handle_Empty_Results()
    {
        // Arrange
        var jobArgs = new AuditLogExportJobArgs
        {
            UserId = Guid.NewGuid(),
            TenantId = null,
            Email = "test@example.com",
            Culture = "en",
            Filter = new ExportAuditLogsInput
            {
                StartTime = DateTime.Now.AddDays(1), // Future date to ensure no results
                EndTime = DateTime.Now.AddDays(2)
            }
        };

        // Act & Assert - should not throw exception
        await _auditLogExportJob.ExecuteAsync(jobArgs);
    }

    [Fact]
    public async Task EntityChangeExportJob_Should_Handle_Empty_Results()
    {
        // Arrange
        var jobArgs = new EntityChangeExportJobArgs
        {
            UserId = Guid.NewGuid(),
            TenantId = null,
            Email = "test@example.com",
            Culture = "en",
            Filter = new ExportEntityChangesInput
            {
                StartDate = DateTime.Now.AddDays(1), // Future date to ensure no results
                EndDate = DateTime.Now.AddDays(2)
            }
        };

        // Act & Assert - should not throw exception
        await _entityChangeExportJob.ExecuteAsync(jobArgs);
    }

    [Fact]
    public async Task ExcelFileDownloadService_Should_Cleanup_Old_Files()
    {
        // Arrange
        var testFileName = "test_cleanup.xlsx";
        await _excelFileDownloadService.SaveFileAsync(testFileName, new MemoryStream(), null);

        // Act
        await _excelFileDownloadService.CleanupOldFilesAsync();

        // Assert
        (await _blobContainer.ExistsAsync(testFileName)).ShouldBeTrue();
        
        // configure options
        var options = GetRequiredService<IOptions<AuditLogExcelFileOptions>>().Value;
        options.FileRetentionHours = 0;

        // Act
        await _excelFileDownloadService.CleanupOldFilesAsync();

        // Assert
        (await _blobContainer.ExistsAsync(testFileName)).ShouldBeFalse();
    }
} 