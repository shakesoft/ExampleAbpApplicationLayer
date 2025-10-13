using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
using Volo.Abp.Users;
using Volo.Abp.DynamicProxy;

namespace Volo.Abp.AuditLogging;

public class AuditLogExportJob : AsyncBackgroundJob<AuditLogExportJobArgs>, ITransientDependency
{
    private const int BatchSize = 10000; // Configurable batch size
    
    protected IAuditLogRepository AuditLogRepository { get; }
    protected IAuditLogExcelExportService ExcelExportService { get; }
    protected IEmailSender EmailSender { get; }
    protected ICurrentTenant CurrentTenant { get; }
    protected ICurrentUser CurrentUser { get; }
    protected IObjectMapper ObjectMapper { get; }
    protected IStringLocalizer<AuditLoggingResource> L { get; }
    protected ITemplateRenderer TemplateRenderer { get; }
    protected IExceptionToErrorInfoConverter ExceptionToErrorInfoConverter { get; }
    protected AbpExceptionHandlingOptions ExceptionHandlingOptions { get; }
    protected IExcelFileDownloadService ExcelFileDownloadService { get; }
    protected AuditLogExcelFileOptions ExcelFileOptions { get; }

    public AuditLogExportJob(
        IAuditLogRepository auditLogRepository,
        IAuditLogExcelExportService excelExportService,
        IEmailSender emailSender,
        ICurrentTenant currentTenant,
        ICurrentUser currentUser,
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
        CurrentUser = currentUser;
        ObjectMapper = objectMapper;
        L = localizer;
        TemplateRenderer = templateRenderer;
        ExceptionToErrorInfoConverter = exceptionToErrorInfoConverter;
        ExceptionHandlingOptions = exceptionHandlingOptions.Value;
        ExcelFileDownloadService = excelFileDownloadService;
        ExcelFileOptions = fileOptions.Value;
    }

    [UnitOfWork]
    public override async Task ExecuteAsync(AuditLogExportJobArgs args)
    {
        using (CurrentTenant.Change(args.TenantId))
        {
            using (CultureHelper.Use(args.Culture))
            {
                try
                {
                    Logger.LogInformation("Starting audit log export for user {UserId}", args.UserId);

                    // Process data in batches to optimize memory usage
                    var allAuditLogs = new List<AuditLog>();
                    var skipCount = 0;
                    var hasMoreData = true;

                    ObjectHelper.TrySetProperty(ProxyHelper.UnProxy(AuditLogRepository).As<IAuditLogRepository>(), x => x.IsChangeTrackingEnabled, () => false);

                    while (hasMoreData)
                    {
                        Logger.LogDebug("Processing batch starting from {SkipCount}", skipCount);

                        var auditLogsBatch = await AuditLogRepository.GetListAsync(
                            sorting: args.Filter.Sorting,
                            maxResultCount: BatchSize,
                            skipCount: skipCount,
                            httpMethod: args.Filter.HttpMethod,
                            httpStatusCode: args.Filter.HttpStatusCode,
                            url: args.Filter.Url,
                            clientId: args.Filter.ClientId,
                            userName: args.Filter.UserName,
                            applicationName: args.Filter.ApplicationName,
                            clientIpAddress: args.Filter.ClientIpAddress,
                            correlationId: args.Filter.CorrelationId,
                            maxExecutionDuration: args.Filter.MaxExecutionDuration,
                            minExecutionDuration: args.Filter.MinExecutionDuration,
                            hasException: args.Filter.HasException,
                            startTime: args.Filter.StartTime,
                            endTime: args.Filter.EndTime,
                            includeDetails: false
                        );

                        if (auditLogsBatch.Count == 0)
                        {
                            hasMoreData = false;
                        }
                        else
                        {
                            allAuditLogs.AddRange(auditLogsBatch);
                            
                            skipCount += BatchSize;
                            
                            // If we got less than batch size, we've reached the end
                            if (auditLogsBatch.Count < BatchSize)
                            {
                                hasMoreData = false;
                            }

                            Logger.LogDebug("Processed batch with {Count} records. Total so far: {Total}", 
                                auditLogsBatch.Count, allAuditLogs.Count);
                        }
                    }

                    // Create file name
                    var fileNameWithoutExt = $"AuditLogs_{DateTime.Now:yyyyMMdd_HHmmss}";
                    var fileName = ExcelFileDownloadService.GenerateUniqueFileName(fileNameWithoutExt, ".xlsx");
                    
                    // Generate Excel file content
                    using (var memoryStream = new MemoryStream())
                    {
                        await ExcelExportService.CreateExcelFileAsync(allAuditLogs, memoryStream);
                        
                        // Save to BLOB storage
                        await ExcelFileDownloadService.SaveFileAsync(fileName, memoryStream, args.UserId);
                    }
                    
                    // Create download link
                    var downloadLink = await ExcelFileDownloadService.CreateDownloadLinkAsync(fileName);

                    // Prepare and send email template
                    var emailSubject = L["AuditLogExportCompletedSubject"];
                    var emailBody = await TemplateRenderer.RenderAsync(
                        AuditLoggingEmailTemplates.AuditLogExportCompleted,
                        new
                        {
                            fileName = fileName,
                            recordCount = allAuditLogs.Count,
                            downloadLink = downloadLink,
                            linkExpirationUtcTime = DateTime.UtcNow.AddHours(ExcelFileOptions.FileRetentionHours).ToString("yyyy-MM-dd HH:mm:ss")
                        }
                    );

                    await EmailSender.SendAsync(
                        args.Email,
                        emailSubject,
                        emailBody
                    );

                    Logger.LogInformation("Audit log export completed successfully for user {UserId}. Records: {Count}", 
                        args.UserId, allAuditLogs.Count);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to export audit logs for user {UserId}", args.UserId);

                    try
                    {
                        // Use ABP's exception to error info converter for safe error message
                        var errorInfo = ExceptionToErrorInfoConverter.Convert(ex, options =>
                        {
                            options.SendExceptionsDetailsToClients = ExceptionHandlingOptions.SendExceptionsDetailsToClients;
                            options.SendStackTraceToClients = ExceptionHandlingOptions.SendStackTraceToClients;
                        });

                        var errorEmailSubject = L["AuditLogExportFailedSubject"];
                        var errorEmailBody = await TemplateRenderer.RenderAsync(
                            AuditLoggingEmailTemplates.AuditLogExportFailed,
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