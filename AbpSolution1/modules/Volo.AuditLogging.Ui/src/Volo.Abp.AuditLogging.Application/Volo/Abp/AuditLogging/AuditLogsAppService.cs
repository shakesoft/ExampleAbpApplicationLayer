using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging.ExcelFileDownload;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Features;
using Volo.Abp.Json;
using Volo.Abp.Content;

namespace Volo.Abp.AuditLogging;

[RequiresFeature(AbpAuditLoggingFeatures.Enable)]
[Authorize(AbpAuditLoggingPermissions.AuditLogs.Default)]
[DisableAuditing]
public class AuditLogsAppService : AuditLogsAppServiceBase, IAuditLogsAppService
{
    private const int ExcelExportThreshold = 1000;
    
    protected IAuditLogRepository AuditLogRepository { get; }
    protected IJsonSerializer JsonSerializer { get; }
    protected IPermissionChecker PermissionChecker { get; }
    protected IPermissionDefinitionManager PermissionDefinitionManager { get; }
    protected IAuditLogExcelExportService ExcelExportService { get; }
    protected IEntityChangeExcelExportService EntityChangeExcelExportService { get; }
    protected IBackgroundJobManager BackgroundJobManager { get; }
    
    protected IExcelFileDownloadService ExcelFileDownloadService { get; }

    public AuditLogsAppService(
        IAuditLogRepository auditLogRepository,
        IJsonSerializer jsonSerializer,
        IPermissionChecker permissionChecker,
        IPermissionDefinitionManager permissionDefinitionManager,
        IAuditLogExcelExportService excelExportService,
        IEntityChangeExcelExportService entityChangeExcelExportService,
        IBackgroundJobManager backgroundJobManager, 
        IExcelFileDownloadService excelFileDownloadService)
    {
        AuditLogRepository = auditLogRepository;
        JsonSerializer = jsonSerializer;
        PermissionChecker = permissionChecker;
        PermissionDefinitionManager = permissionDefinitionManager;
        ExcelExportService = excelExportService;
        EntityChangeExcelExportService = entityChangeExcelExportService;
        BackgroundJobManager = backgroundJobManager;
        ExcelFileDownloadService = excelFileDownloadService;
    }

    public virtual async Task<PagedResultDto<AuditLogDto>> GetListAsync(GetAuditLogListDto input)
    {
        var auditLogs = await AuditLogRepository.GetListAsync(
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount,
            httpMethod: input.HttpMethod,
            httpStatusCode: input.HttpStatusCode,
            url: input.Url,
            clientId: input.ClientId,
            userName: input.UserName,
            applicationName: input.ApplicationName,
            clientIpAddress: input.ClientIpAddress,
            correlationId: input.CorrelationId,
            maxExecutionDuration: input.MaxExecutionDuration,
            minExecutionDuration: input.MinExecutionDuration,
            hasException: input.HasException,
            startTime: input.StartTime,
            endTime: input.EndTime,
            includeDetails: false
        );

        var totalCount = await AuditLogRepository.GetCountAsync(
            httpMethod: input.HttpMethod,
            httpStatusCode: input.HttpStatusCode,
            url: input.Url,
            clientId: input.ClientId,
            userName: input.UserName,
            applicationName: input.ApplicationName,
            clientIpAddress: input.ClientIpAddress,
            correlationId: input.CorrelationId,
            maxExecutionDuration: input.MaxExecutionDuration,
            minExecutionDuration: input.MinExecutionDuration,
            hasException: input.HasException,
            startTime: input.StartTime,
            endTime: input.EndTime
        );

        var dtos = ObjectMapper.Map<List<AuditLog>, List<AuditLogDto>>(auditLogs);

        return new PagedResultDto<AuditLogDto>(totalCount, dtos);
    }

    public virtual async Task<AuditLogDto> GetAsync(Guid id)
    {
        var log = await AuditLogRepository.GetAsync(id);
        var logDto = ObjectMapper.Map<AuditLog, AuditLogDto>(log);

        foreach (var action in logDto.Actions)
        {
            if (action.Parameters.IsNullOrEmpty())
            {
                action.Parameters = "{}";
            }

            var parsedJson = JsonSerializer.Deserialize<object>(action.Parameters);
            action.Parameters = JsonSerializer.Serialize(parsedJson, indented: true);
        }

        return logDto;
    }

    public virtual async Task<GetErrorRateOutput> GetErrorRateAsync(GetErrorRateFilter filter)
    {
        var successfulLogCount = await AuditLogRepository.GetCountAsync(
            startTime: filter.StartDate,
            endTime: filter.EndDate.AddDays(1),
            hasException: false
        );
        var errorLogCount = await AuditLogRepository.GetCountAsync(startTime:
            filter.StartDate,
            endTime: filter.EndDate.AddDays(1),
            hasException: true
        );

        return new GetErrorRateOutput
        {
            Data = new Dictionary<string, long>
                {
                    {L["Fault"], errorLogCount},
                    {L["Success"], successfulLogCount}
                }
        };
    }

    public virtual async Task<GetAverageExecutionDurationPerDayOutput> GetAverageExecutionDurationPerDayAsync(GetAverageExecutionDurationPerDayInput filter)
    {
        var result =
            await AuditLogRepository.GetAverageExecutionDurationPerDayAsync(filter.StartDate, filter.EndDate);

        return new GetAverageExecutionDurationPerDayOutput
        {
            Data = result.ToDictionary(x => x.Key.ToString("d"), x => x.Value)
        };
    }

    public virtual async Task<PagedResultDto<EntityChangeDto>> GetEntityChangesAsync(GetEntityChangesDto input)
    {
        var entityChanges = await AuditLogRepository.GetEntityChangeListAsync(
                                                    input.Sorting,
                                                    input.MaxResultCount,
                                                    input.SkipCount,
                                                    input.AuditLogId,
                                                    input.StartDate,
                                                    input.EndDate,
                                                    input.EntityChangeType,
                                                    input.EntityId,
                                                    input.EntityTypeFullName, true);

        var entityChangesCount = await AuditLogRepository.GetEntityChangeCountAsync(input.AuditLogId,
                                                                                    input.StartDate,
                                                                                    input.EndDate,
                                                                                    input.EntityChangeType,
                                                                                    input.EntityId,
                                                                                    input.EntityTypeFullName);

        var dtos = ObjectMapper.Map<List<EntityChange>, List<EntityChangeDto>>(entityChanges);

        return new PagedResultDto<EntityChangeDto>(entityChangesCount, dtos);
    }

    [AllowAnonymous]
    public virtual async Task<List<EntityChangeWithUsernameDto>> GetEntityChangesWithUsernameAsync(EntityChangeFilter input)
    {
        await CheckPermissionForEntity(input.EntityTypeFullName);
        var entityChanges = await AuditLogRepository.GetEntityChangesWithUsernameAsync(input.EntityId, input.EntityTypeFullName);

        return ObjectMapper.Map<List<EntityChangeWithUsername>, List<EntityChangeWithUsernameDto>>(entityChanges);
    }

    public virtual async Task<EntityChangeWithUsernameDto> GetEntityChangeWithUsernameAsync(Guid entityChangeId)
    {
        var entityChange = await AuditLogRepository.GetEntityChangeWithUsernameAsync(entityChangeId);
        await CheckPermissionForEntity(entityChange.EntityChange.EntityTypeFullName);

        return ObjectMapper.Map<EntityChangeWithUsername, EntityChangeWithUsernameDto>(entityChange);
    }

    public virtual async Task<EntityChangeDto> GetEntityChangeAsync(Guid entityChangeId)
    {
        var entityChange = await AuditLogRepository.GetEntityChange(entityChangeId);
        await CheckPermissionForEntity(entityChange.EntityTypeFullName);

        var entityChangeDto = ObjectMapper.Map<EntityChange, EntityChangeDto>(entityChange);
        return entityChangeDto;
    }

    [Authorize(AbpAuditLoggingPermissions.AuditLogs.Export)]
    public virtual async Task<ExportAuditLogsOutput> ExportToExcelAsync(ExportAuditLogsInput input)
    {
        // First, get the count to determine if we need background job
        var totalCount = await AuditLogRepository.GetCountAsync(
            httpMethod: input.HttpMethod,
            httpStatusCode: input.HttpStatusCode,
            url: input.Url,
            clientId: input.ClientId,
            userName: input.UserName,
            applicationName: input.ApplicationName,
            clientIpAddress: input.ClientIpAddress,
            correlationId: input.CorrelationId,
            maxExecutionDuration: input.MaxExecutionDuration,
            minExecutionDuration: input.MinExecutionDuration,
            hasException: input.HasException,
            startTime: input.StartTime,
            endTime: input.EndTime
        );

        if (totalCount > ExcelExportThreshold)
        {
            // Use background job for large datasets and send via email
            var jobArgs = new AuditLogExportJobArgs
            {
                UserId = CurrentUser.Id,
                TenantId = CurrentTenant.Id,
                Email = CurrentUser.Email,
                Culture = CultureInfo.CurrentCulture.Name,
                Filter = input
            };

            await BackgroundJobManager.EnqueueAsync(jobArgs);

            return new ExportAuditLogsOutput
            {
                Message = L["ExportJobQueued", totalCount],
                IsBackgroundJob = true
            };
        }

        // Process immediately for small datasets
        var auditLogs = await AuditLogRepository.GetListAsync(
            sorting: input.Sorting,
            maxResultCount: (int)totalCount,
            skipCount: 0,
            httpMethod: input.HttpMethod,
            httpStatusCode: input.HttpStatusCode,
            url: input.Url,
            clientId: input.ClientId,
            userName: input.UserName,
            applicationName: input.ApplicationName,
            clientIpAddress: input.ClientIpAddress,
            correlationId: input.CorrelationId,
            maxExecutionDuration: input.MaxExecutionDuration,
            minExecutionDuration: input.MinExecutionDuration,
            hasException: input.HasException,
            startTime: input.StartTime,
            endTime: input.EndTime,
            includeDetails: false
        );
            
        // Export to file
        var excelStream = await ExcelExportService.ExportToExcelStreamAsync(auditLogs);
            
        return new ExportAuditLogsOutput
        {
            Message = L["ExportReady", totalCount],
            FileData = await excelStream.GetAllBytesAsync(),
            FileName = $"AuditLogs_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
            IsBackgroundJob = false
        };
    }

    [Authorize(AbpAuditLoggingPermissions.AuditLogs.Export)]
    public async Task<IRemoteStreamContent> DownloadExcelAsync(string fileName)
    {
        var fileStream = await ExcelFileDownloadService.GetFileContentAsync(fileName);
        if (fileStream == null)
        {
            throw new FileNotFoundException(L["FileNotFound", fileName]);
        }
        
        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        return new RemoteStreamContent(fileStream, fileName, contentType);
    }

    [Authorize(AbpAuditLoggingPermissions.AuditLogs.Export)]
    public virtual async Task<ExportEntityChangesOutput> ExportEntityChangesToExcelAsync(ExportEntityChangesInput input)
    {
        // First, get the count to determine if we need background job
        var totalCount = await AuditLogRepository.GetEntityChangeCountAsync(
            null, // auditLogId
            input.StartDate,
            input.EndDate,
            input.EntityChangeType,
            input.EntityId,
            input.EntityTypeFullName
        );

        if (totalCount > ExcelExportThreshold)
        {
            // Use background job for large datasets and send via email
            var jobArgs = new EntityChangeExportJobArgs
            {
                UserId = CurrentUser.Id,
                TenantId = CurrentTenant.Id,
                Email = CurrentUser.Email,
                Culture = CultureInfo.CurrentCulture.Name,
                Filter = input
            };

            await BackgroundJobManager.EnqueueAsync(jobArgs);

            return new ExportEntityChangesOutput
            {
                Message = L["ExportJobQueued", totalCount],
                IsBackgroundJob = true
            };
        }

        // Process immediately for small datasets and return file data
        var entityChanges = await AuditLogRepository.GetEntityChangeListAsync(
            input.Sorting,
            (int)totalCount,
            0,
            null, // auditLogId
            input.StartDate,
            input.EndDate,
            input.EntityChangeType,
            input.EntityId,
            input.EntityTypeFullName,
            false // includeDetails
        );

        var excelBytes = await EntityChangeExcelExportService.ExportToExcelStreamAsync(entityChanges);
        var fileName = $"EntityChanges_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        return new ExportEntityChangesOutput
        {
            Message = L["ExportReady", totalCount],
            FileData = await excelBytes.GetAllBytesAsync(),
            FileName = fileName,
            IsBackgroundJob = false
        };
    }

    protected virtual async Task CheckPermissionForEntity(string entityFullName)
    {
        var permissionName = $"AuditLogging.ViewChangeHistory:{entityFullName}";

        var permission = await PermissionDefinitionManager.GetOrNullAsync(permissionName);

        if (permission == null)
        {
            await AuthorizationService.CheckAsync(AbpAuditLoggingPermissions.AuditLogs.Default);
        }
        else
        {
            if (!(await PermissionChecker.IsGrantedAsync(permissionName)))
            {
                await AuthorizationService.CheckAsync(AbpAuditLoggingPermissions.AuditLogs.Default);
            }
        }
    }
}
