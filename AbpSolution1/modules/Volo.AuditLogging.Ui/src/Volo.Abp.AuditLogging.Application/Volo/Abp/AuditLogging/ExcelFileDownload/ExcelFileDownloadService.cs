using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;
using Volo.Abp.AuditLogging;
using Microsoft.Extensions.Logging;
using Volo.Abp.Guids;

namespace Volo.Abp.AuditLogging.ExcelFileDownload;

/// <summary>
/// File download service implementation
/// </summary>
public class ExcelFileDownloadService : IExcelFileDownloadService, ITransientDependency
{
    private readonly AuditLogExcelFileOptions _options;
    private readonly IBlobContainer<AuditLogExcelContainer> _blobContainer;
    private readonly IAuditLogExcelFileRepository _excelFileRepository;
    private readonly IClock _clock;
    private readonly ILogger<ExcelFileDownloadService> _logger;
    private readonly IGuidGenerator _guidGenerator;
    
    public ExcelFileDownloadService(
        IOptions<AuditLogExcelFileOptions> options,
        IBlobContainer<AuditLogExcelContainer> blobContainer,
        IAuditLogExcelFileRepository excelFileRepository,
        IClock clock,
        ILogger<ExcelFileDownloadService> logger,
        IGuidGenerator guidGenerator)
    {
        _options = options.Value;
        _blobContainer = blobContainer;
        _excelFileRepository = excelFileRepository;
        _clock = clock;
        _logger = logger;
        _guidGenerator = guidGenerator;
    }
    
    public Task<string> CreateDownloadLinkAsync(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be empty", nameof(fileName));
        }
        
        return Task.FromResult($"{_options.DownloadBaseUrl.EnsureEndsWith('/')}api/audit-logging/audit-logs/download-excel/{fileName}");
    }
    
    public async Task<Stream> GetFileContentAsync(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be empty", nameof(fileName));
        }
        
        return await _blobContainer.GetOrNullAsync(fileName);
    }
    
    public string GenerateUniqueFileName(string fileNameWithoutExtension, string extension)
    {
        // Add GUID to make file name unique
        var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
        return $"{fileNameWithoutExtension}_{uniqueId}{extension}";
    }
    
    public async Task SaveFileAsync(string fileName, Stream fileContent, Guid? createdBy)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be empty", nameof(fileName));
        }
        
        if (fileContent == null)
        {
            throw new ArgumentException("File content cannot be empty", nameof(fileContent));
        }
        
        // Save to BLOB storage
        await _blobContainer.SaveAsync(fileName, fileContent);
        
        // Save metadata to database
        var excelFile = new AuditLogExcelFile(
            _guidGenerator.Create(),
            fileName,
            creatorId: createdBy
        );
        
        await _excelFileRepository.InsertAsync(excelFile);
    }
    
    public async Task CleanupOldFilesAsync()
    {
        var retentionDate = _clock.Now.AddHours(-_options.FileRetentionHours);
        
        // Get old files from database
        var oldFiles = await _excelFileRepository.GetListCreationTimeBeforeAsync(
            retentionDate,
            maxResultCount: 100
        );
        
        foreach (var file in oldFiles)
        {
            try
            {
                // Delete from blob storage
                await _blobContainer.DeleteAsync(file.FileName);
                
                // Delete from database
                await _excelFileRepository.DeleteAsync(file);
            }
            catch (Exception ex)
            {
                // Log error but continue with next file
                _logger.LogError(ex, "Failed to delete old excel file: {FileName}", file.FileName);
            }
        }
    }
} 