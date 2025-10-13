using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AuditLogging.ExcelFileDownload;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.AuditLogging.Localization;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.TextTemplating;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace Volo.Abp.AuditLogging;

public class EntityChangeExportJob : AsyncBackgroundJob<EntityChangeExportJobArgs>, ITransientDependency
{
    private const int BatchSize = 10000; // Configurable batch size
    
    protected IAuditLogRepository AuditLogRepository { get; }
    protected IEntityChangeExcelExportService ExcelExportService { get; }
    protected IEmailSender EmailSender { get; }
    protected ICurrentTenant CurrentTenant { get; }
    protected IObjectMapper ObjectMapper { get; }
    protected IStringLocalizer<AuditLoggingResource> L { get; }
    protected ITemplateRenderer TemplateRenderer { get; }
    protected IExceptionToErrorInfoConverter ExceptionToErrorInfoConverter { get; }
    protected AbpExceptionHandlingOptions ExceptionHandlingOptions { get; }
    
    protected IExcelFileDownloadService ExcelFileDownloadService { get; }
    
    protected AuditLogExcelFileOptions ExcelFileOptions { get; }

    public EntityChangeExportJob(
        IAuditLogRepository auditLogRepository,
        IEntityChangeExcelExportService excelExportService,
        IEmailSender emailSender,
        ICurrentTenant currentTenant,
        IObjectMapper objectMapper,
        IStringLocalizer<AuditLoggingResource> localizer,
        ITemplateRenderer templateRenderer,
        IExceptionToErrorInfoConverter exceptionToErrorInfoConverter,
        IOptions<AbpExceptionHandlingOptions> exceptionHandlingOptions, 
        IExcelFileDownloadService excelFileDownloadService,
        IOptions<AuditLogExcelFileOptions> fileOptions)
    {
        AuditLogRepository = auditLogRepository;
        ExcelExportService = excelExportService;
        EmailSender = emailSender;
        CurrentTenant = currentTenant;
        ObjectMapper = objectMapper;
        L = localizer;
        TemplateRenderer = templateRenderer;
        ExceptionToErrorInfoConverter = exceptionToErrorInfoConverter;
        ExcelFileDownloadService = excelFileDownloadService;
        ExceptionHandlingOptions = exceptionHandlingOptions.Value;
        ExcelFileOptions = fileOptions.Value;
    }

    [UnitOfWork]
    public override async Task ExecuteAsync(EntityChangeExportJobArgs args)
    {
        using (CurrentTenant.Change(args.TenantId))
        {
            using (CultureHelper.Use(args.Culture))
            {
                try
                {
                    Logger.LogInformation("Starting entity change export for user {UserId}", args.UserId);

                    // Process data in batches to optimize memory usage
                    var allEntityChanges = new List<EntityChange>();
                    var skipCount = 0;
                    var hasMoreData = true;

                    ObjectHelper.TrySetProperty(AuditLogRepository, x => x.IsChangeTrackingEnabled, () => false);

                    while (hasMoreData)
                    {
                        Logger.LogDebug("Processing batch starting from {SkipCount}", skipCount);

                        var entityChangesBatch = await AuditLogRepository.GetEntityChangeListAsync(
                            args.Filter.Sorting,
                            BatchSize,
                            skipCount,
                            null, // auditLogId
                            args.Filter.StartDate,
                            args.Filter.EndDate,
                            args.Filter.EntityChangeType,
                            args.Filter.EntityId,
                            args.Filter.EntityTypeFullName,
                            false // includeDetails
                        );

                        if (entityChangesBatch.Count == 0)
                        {
                            hasMoreData = false;
                        }
                        else
                        {
                            allEntityChanges.AddRange(entityChangesBatch);
                            
                            skipCount += BatchSize;
                            
                            // If we got less than batch size, we've reached the end
                            if (entityChangesBatch.Count < BatchSize)
                            {
                                hasMoreData = false;
                            }

                            Logger.LogDebug("Processed batch with {Count} records. Total so far: {Total}", 
                                entityChangesBatch.Count, allEntityChanges.Count);
                        }
                    }
                    
                    // Create file name
                    var fileNameWithoutExt = $"EntityChanges_{DateTime.Now:yyyyMMdd_HHmmss}";
                    var fileName = ExcelFileDownloadService.GenerateUniqueFileName(fileNameWithoutExt, ".xlsx");
                    
                    // Generate Excel file and save to BLOB storage
                    using (var memoryStream = new MemoryStream())
                    {
                        await ExcelExportService.CreateExcelFileAsync(allEntityChanges, memoryStream);
                        
                        // Save to BLOB storage
                        await ExcelFileDownloadService.SaveFileAsync(fileName, memoryStream, args.UserId);
                    }
                    
                    // Create download link
                    var downloadLink = await ExcelFileDownloadService.CreateDownloadLinkAsync(fileName);

                    // Use ABP's template renderer with localized subject
                    var emailSubject = L["EntityChangeExportCompletedSubject"];
                    var emailBody = await TemplateRenderer.RenderAsync(
                        AuditLoggingEmailTemplates.EntityChangeExportCompleted,
                        new
                        {
                            fileName = fileName,
                            recordCount = allEntityChanges.Count,
                            downloadLink = downloadLink,
                            linkExpirationUtcTime = DateTime.UtcNow.AddHours(ExcelFileOptions.FileRetentionHours).ToString("yyyy-MM-dd HH:mm:ss")
                        }
                    );

                    await EmailSender.SendAsync(
                        args.Email,
                        emailSubject,
                        emailBody
                    );

                    Logger.LogInformation("Entity change export completed successfully for user {UserId}. Records: {Count}",
                        args.UserId, allEntityChanges.Count);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to export entity changes for user {UserId}", args.UserId);

                    try
                    {
                        // Use ABP's exception to error info converter for safe error message
                        var errorInfo = ExceptionToErrorInfoConverter.Convert(ex, options =>
                        {
                            options.SendExceptionsDetailsToClients = ExceptionHandlingOptions.SendExceptionsDetailsToClients;
                            options.SendStackTraceToClients = ExceptionHandlingOptions.SendStackTraceToClients;
                        });

                        var errorEmailSubject = L["EntityChangeExportFailedSubject"];
                        var errorEmailBody = await TemplateRenderer.RenderAsync(
                            AuditLoggingEmailTemplates.EntityChangeExportFailed,
                            new
                            {
                                errorMessage = errorInfo.Message
                            }
                        );

                        await EmailSender.SendAsync(
                            args.Email,
                            errorEmailSubject,
                            errorEmailBody
                        );
                    }
                    catch (Exception emailEx)
                    {
                        Logger.LogError(emailEx, "Failed to send error notification email for user {UserId}", args.UserId);
                    }
                }
            }
        }
    }
} 